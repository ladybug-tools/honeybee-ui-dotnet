import React, { useContext, useEffect, useRef, useState } from "react";
import type { GetRef, InputRef, TablePaginationConfig } from "antd";
import { Button, Flex, Form, Input, Popconfirm, Table, Tooltip } from "antd";
import { PlusOutlined, DeleteOutlined } from "@ant-design/icons";
import { FilterValue, SorterResult } from "antd/es/table/interface";
import Paragraph from "antd/es/typography/Paragraph";

type FormInstance<T> = GetRef<typeof Form<T>>;

const EditableContext = React.createContext<FormInstance<any> | null>(null);

interface Item {
  key: string;
  name: string;
  age: string;
  address: string;
}

interface EditableRowProps {
  index: number;
}

const EditableRow: React.FC<EditableRowProps> = ({ index, ...props }) => {
  const [form] = Form.useForm();
  return (
    <Form form={form} component={false}>
      <EditableContext.Provider value={form}>
        <tr {...props} />
      </EditableContext.Provider>
    </Form>
  );
};

interface EditableCellProps {
  title: React.ReactNode;
  editable: boolean;
  dataIndex: keyof Item;
  record: Item;
  handleSave: (record: Item) => void;
}

const EditableCell: React.FC<React.PropsWithChildren<EditableCellProps>> = ({
  title,
  editable,
  children,
  dataIndex,
  record,
  handleSave,
  ...restProps
}) => {
  const [editing, setEditing] = useState(false);
  const inputRef = useRef<InputRef>(null);
  const form = useContext(EditableContext)!;

  useEffect(() => {
    if (editing) {
      inputRef.current?.focus();
    }
  }, [editing]);

  const toggleEdit = () => {
    setEditing(!editing);
    form.setFieldsValue({ [dataIndex]: record[dataIndex] });
  };

  const save = async () => {
    try {
      const values = await form.validateFields();

      toggleEdit();
      handleSave({ ...record, ...values });
    } catch (errInfo) {
      console.log("Save failed:", errInfo);
    }
  };

  let childNode = children;

  if (editable) {
    childNode = editing ? (
      <Form.Item
        style={{ margin: 0 }}
        name={dataIndex}
        rules={[
          {
            required: true,
            message: `${title} is required.`,
          },
        ]}
      >
        <Input ref={inputRef} onPressEnter={save} onBlur={save} />
      </Form.Item>
    ) : (
      <div
        className="editable-cell-value-wrap"
        style={{ paddingRight: 24 }}
        onClick={toggleEdit}
      >
        {children}
      </div>
    );
  }

  return <td {...restProps}>{childNode}</td>;
};

type EditableTableProps = Parameters<typeof Table>[0];

interface DataType {
  key: React.Key;
  name: string;
  url: string;
}

type ColumnTypes = Exclude<EditableTableProps["columns"], undefined>;

const TableEditableCell: React.FC<{
  value?: string[];
  onChange: (data: string[]) => void;
}> = ({ value = [], onChange }) => {
  const initData: DataType[] = value.map((_, i) => ({
    key: i,
    name: i.toString(),
    url: _,
  }));
  const [dataSource, setDataSource] = useState(initData);

  const [count, setCount] = useState(2);

  const handleDelete = (key: React.Key) => {
    const newData = dataSource.filter((item) => item.key !== key);
    setDataSource(newData);
    onChange(newData.map((_) => _.url));
  };

  const truncateMiddle = (text:string, maxLength:number) => {
    if (text.length <= maxLength) return text;
    const half = Math.floor(maxLength / 4);
    return text.slice(0, half) + ' ... ' + text.slice(-half*3);
  };

  const EllipsisMiddle: React.FC<{ suffixCount: number; children: string }> = ({
    suffixCount,
    children,
  }) => {
    const start = children.slice(0, children.length - suffixCount);
    const suffix = children.slice(-suffixCount).trim();
    return (
      <Paragraph style={{ maxWidth: '100%' }} ellipsis={{ suffix }} title="Click to edit">
        {start}
      </Paragraph>
    );
  };

  const defaultColumns: (ColumnTypes[number] & {
    editable?: boolean;
    dataIndex: string;
  })[] = [
    // {
    //   dataIndex: "name",
    //   width: "30%",
    // },
    {
      dataIndex: "url",
      editable: true,
      render: (_, r) =>
        <EllipsisMiddle suffixCount={40}>{_}</EllipsisMiddle>
    },
    {
      dataIndex: "operation",
      width: "5%",
      render: (_, record) =>
        dataSource.length >= 1 ? (
          <Popconfirm
            title="Sure to delete?"
            onConfirm={() => handleDelete(record.key)}
          >
            <Tooltip title="Delete" placement="left">
              <Button
                shape="circle"
                icon={<DeleteOutlined />}
                // onClick={handleAdd}
              />
            </Tooltip>
          </Popconfirm>
        ) : null,
    },
  ];

  const handleAdd = () => {
    const newData: DataType = {
      key: count,
      name: `New Location ${count}`,
      url: `New Weather URL. ${count}`,
    };
    const newList = [...dataSource, newData];
    setDataSource(newList);
    setCount(count + 1);
    onChange(newList.map((_) => _.url));
  };

  const handleSave = (row: DataType) => {
    const newData = [...dataSource];
    const index = newData.findIndex((item) => row.key === item.key);
    const item = newData[index];
    newData.splice(index, 1, {
      ...item,
      ...row,
    });
    setDataSource(newData);
    onChange(newData.map((_) => _.url));
  };

  const components = {
    body: {
      row: EditableRow,
      cell: EditableCell,
    },
  };

  const columns = defaultColumns.map((col) => {
    if (!col.editable) {
      return col;
    }
    return {
      ...col,
      onCell: (record: DataType) => ({
        record,
        editable: col.editable,
        dataIndex: col.dataIndex,
        title: col.title,
        handleSave,
      }),
    };
  });

  const handleTableChange = (
    pagination: TablePaginationConfig,
    filters: Record<string, FilterValue | null>,
    sorter: SorterResult<any> | SorterResult<any>[],
    extra: {
      currentDataSource: any[];
      action: "paginate" | "sort" | "filter";
    }
  ) => {
    // this is never called
    console.log("called inside Table:", extra.currentDataSource);
    onChange(extra.currentDataSource.map((item) => item.value));
  };

  return (
    <Table
        components={components}
        // rowClassName={() => "editable-row"}
        // bordered
        dataSource={dataSource}
        columns={columns as ColumnTypes}
        pagination={false}
        showHeader={false}
        footer={() => (
          // <Button
          //   onClick={handleAdd}
          //   type="primary"
          //   // style={{ marginBottom: 16 }}
          // >
          //   Add a row
          // </Button>
          <Flex justify="flex-end">
            <Tooltip title="Add a new weather" placement="left">
              <Button
                shape="circle"
                icon={<PlusOutlined />}
                onClick={handleAdd}
              />
            </Tooltip>
          </Flex>
        )}
        onChange={handleTableChange}
      />
  );
};

export default TableEditableCell;
