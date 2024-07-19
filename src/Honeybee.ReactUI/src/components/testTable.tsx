import { Table, TablePaginationConfig, TableProps } from "antd";
import { FilterValue } from "antd/es/table/interface";

interface DataType {
  key: string;
  value: string;
}

const MyComponent: React.FC<{
  value?: string[];
  onChange: (data: string[]) => void;
}> = ({ value = [], onChange }) => {
  const dataSource: DataType[] = value.map((item, index) => ({
    key: index.toString(),
    value: item,
  }));

  const columns = [
    {
      title: "Value",
      dataIndex: "value",
      key: "value",
    },
  ];

  const handleTableChange: TableProps<DataType>["onChange"] = (
    pagination: TablePaginationConfig,
    filters: Record<string, FilterValue | null>,
    sorter: any,
    extra: {
      currentDataSource: DataType[];
      action: "paginate" | "sort" | "filter";
    }
  ) => {
    onChange(extra.currentDataSource.map((item) => item.value));
  };

  return (
    <Table
      columns={columns}
      dataSource={dataSource}
      onChange={handleTableChange}
      rowKey="key"
    />
  );
};
export default MyComponent;
