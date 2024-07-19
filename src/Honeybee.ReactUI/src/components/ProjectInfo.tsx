import {
  Button,
  Col,
  DatePicker,
  Flex,
  Form,
  Input,
  InputNumber,
  InputNumberProps,
  Row,
  Select,
  Slider,
  Space,
  Switch,
  Table,
  TableColumnsType,
  Tabs,
  TabsProps,
} from "antd";
import React, { CSSProperties, useRef, useState } from "react";
import TableEditableCell from "./TableEditableCell";
import {
  Vintages,
  BuildingTypes,
  ClimateZones,
  ProjectInfo,
  Location,
  EfficiencyStandards,
} from "honeybee-schema-sdk";
import { GetEnumKeys } from "../utilities/utility";
import { InfoCircleOutlined, AimOutlined } from "@ant-design/icons";
import { SchemaEditor } from "./SchemaEditor";

const fullWidthStyle: CSSProperties = {
  width: "100%",
};

interface CustomBindingProps {
  name: string;
  [key: string]: any; // Allow any other props
}

const NorthInput: React.FC<CustomBindingProps> = ({ name }) => {
  // const [inputValue, setInputValue] = useState(0);

  // const onChange: InputNumberProps["onChange"] = (value) => {
  //   if (isNaN(value as number)) {
  //     return;
  //   }
  //   setInputValue(value as number);
  // };

  return (
    <Row>
      <Col span={18}>
        <Form.Item name={name}>
          <Slider min={0} max={360} step={0.1} />
        </Form.Item>
      </Col>
      <Col span={5}>
        <Form.Item name={name}>
          <InputNumber
            min={0}
            max={360}
            style={{ margin: "0 16px", width: "100%" }}
            step={0.1}
          />
        </Form.Item>
      </Col>
    </Row>
  );
};

const NumberWithSwitch: React.FC = ({ ...props }) => {
  const [sliderValue, setSliderValue] = useState(0);
  const [autoCal, setSwitchChecked] = useState(true);
  const onSelectChange = (selectedItem: boolean) => {
    // const isAuto = selectedItem === "Auto";
    setSwitchChecked(selectedItem);
  };

  const onSliderChange = (value: number | null) => {
    if (value) setSliderValue(value);
  };
  // const OPTIONS = GetEnumKeys(BuildingTypes);
  // const [selectedItems, setSelectedItems] = useState<string[]>([]);

  // const filteredOptions = OPTIONS.filter((o) => !selectedItems.includes(o));

  const selectBefore = (
    <Select
      style={{ width: "150px" }}
      defaultValue={true}
      onChange={onSelectChange}
      options={[
        { value: false, label: "Manual Input" },
        { value: true, label: "Auto Calculate" },
      ]}
    />
  );

  return (
    <Space.Compact style={{ width: "100%" }}>
      <Form.Item name="slider" noStyle>
        <InputNumber
          addonBefore={selectBefore}
          style={fullWidthStyle}
          min={-12}
          max={14}
          onChange={onSliderChange}
          value={sliderValue}
          disabled={autoCal}
        />
      </Form.Item>
    </Space.Compact>
  );
};

const BuildingType: React.FC = ({ ...props }) => {
  const OPTIONS = Object.values(BuildingTypes).map((item) => ({
    value: item,
    label: item,
  }));
  // const OPTIONS = GetEnumKeys(BuildingTypes);
  // const [selectedItems, setSelectedItems] = useState<string[]>([]);

  // const filteredOptions = OPTIONS.filter((o) => !selectedItems.includes(o));

  return (
    <Select
      {...props}
      mode="multiple"
      // value={selectedItems}
      // onChange={setSelectedItems}
      options={OPTIONS}
    />
  );
};

const BuildingVintage: React.FC = ({ ...props }) => {
  const OPTIONS = Object.values(Vintages).map((item) => ({
    value: item,
    label: item,
  }));
  // const [selectedItems, setSelectedItems] = useState<string[]>([]);

  // const filteredOptions = OPTIONS.filter((o) => !selectedItems.includes(o));
  // const cc = Vintages.ASHRAE_2019;

  return (
    <Select
      {...props}
      mode="multiple"
      // value={selectedItems}
      // onChange={setSelectedItems}
      options={OPTIONS}
    />
  );
};

export const ProjectInfoView: React.FC = () => {
  const dataModel = new ProjectInfo();
  dataModel.ashrae_climate_zone = ClimateZones._6B;
  dataModel.north = 12;
  dataModel.weather_urls = [
    "https://energyplus-weather.s3.amazonaws.com/north_and_central_america_wmo_region_4/USA/MA/USA_MA_Boston-Logan.Intl.AP.725090_TMY3/USA_MA_Boston-Logan.Intl.AP.725090_TMY3.zip",
    "https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/USA_United_States_of_America/NY_New_York/USA_NY_New.York-Kennedy.Intl.AP.744860_TMYx.zip",
  ];
  dataModel.building_type = [
    BuildingTypes.Hospital,
    BuildingTypes.Courthouse,
    BuildingTypes.FullServiceRestaurant,
  ];
  dataModel.vintage = [
    EfficiencyStandards.ASHRAE_2004,
    EfficiencyStandards.ASHRAE_2007,
  ];
  dataModel.location = new Location();
  dataModel.location.city = "My city";
  dataModel.location.longitude = 15.66;
  dataModel.location.latitude = 66.66;
  dataModel.location.elevation = 22.22;

  dataModel.location.time_zone = 5;
  dataModel.location.source = "new source";
  dataModel.location.station_id = "new station id";

  const [form] = Form.useForm();
  const [formData, setFormData] = useState(dataModel);

  const onFinish = (values: ProjectInfo) => {
    console.log("Form values:", values);
    setFormData(values);
  };

  const handleTableChange = (data: string[]) => {
    form.setFieldsValue({ weather_urls: data });
    console.log("Form values:", data);
  };
  const TabPage1: React.FC = () => {
    return (
      <>
        <Form.Item label="North">
          <NorthInput name="north" />
        </Form.Item>
        <Form.Item label="Weather Urls/Files" name="weather_urls">
          {/* <Table dataSource={transformedData} columns={columns} /> */}
          <TableEditableCell onChange={handleTableChange} />
        </Form.Item>
        <Form.Item label="Building Type" name="building_type">
          <BuildingType />
        </Form.Item>
        <Form.Item label="Building Vintage" name="vintage">
          <BuildingVintage />
        </Form.Item>
      </>
    );
  };

  const TabPage2: React.FC = () => {
    const climateZoneOptions = Object.values(ClimateZones).map((cz) => ({
      value: cz,
      label: `ASHRAE ${cz}`,
    }));

    return (
      <>
        <Form.Item label="Climate Zone" name={"ashrae_climate_zone"}>
          <Select
            // defaultValue={V_OPTIONS[0]}
            style={{ textAlign: "left" }}
            // onChange={handleChange}
            options={climateZoneOptions}
          />
        </Form.Item>

        <Form.Item label="City" name={["location", "city"]}>
          <Input />
        </Form.Item>
        <Form.Item label="Latitude" name={["location", "latitude"]}>
          <InputNumber style={fullWidthStyle} />
        </Form.Item>
        <Form.Item label="Longitude" name={["location", "longitude"]}>
          <InputNumber style={fullWidthStyle} />
        </Form.Item>
        <Form.Item label="Elevation" name={["location", "elevation"]}>
          <InputNumber style={fullWidthStyle} />
        </Form.Item>
        <Form.Item label="Time Zone" name={["location", "time_zone"]}>
          <NumberWithSwitch />
        </Form.Item>
        <Form.Item label="Source" name={["location", "source"]}>
          <Input />
        </Form.Item>
        <Form.Item label="Station Id" name={["location", "station_id"]}>
          <Input />
        </Form.Item>
      </>
    );
  };

  const tabItems: TabsProps["items"] = [
    {
      key: "1",
      label: "Information",
      children: <TabPage1 />,
      icon: <InfoCircleOutlined />,
      forceRender: true,
    },
    {
      key: "2",
      label: "Location",
      children: <TabPage2 />,
      icon: <AimOutlined />,
      forceRender: true,
    },
  ];

  const getFormDataToJson = async () => {
    const values = await form.validateFields();
    const jsonString = JSON.stringify(values, null, 2); // Pretty print with 2 spaces
    return jsonString;
  };

  const handleSchemaDataReturn = (json: string) => {
    console.log(json);
    const jObj = JSON.parse(json);
    const hbObj = ProjectInfo.fromJS(jObj);
    form.setFieldsValue(hbObj);
  };

  return (
    <>
      <Form
        form={form}
        name="wrap"
        labelCol={{ span: 6 }}
        wrapperCol={{ span: 18 }}
        layout="horizontal"
        labelAlign="left"
        labelWrap
        // colon={false}
        // style={{ maxWidth: 500 }}
        size="small"
        initialValues={formData}
        onFinish={onFinish}
      >
        <Tabs defaultActiveKey="1" items={tabItems} />

        <Row>
          <Col span={8}>
            <Flex justify="flex-start" gap="small">
              <SchemaEditor
                getData={getFormDataToJson}
                onClose={handleSchemaDataReturn}
              ></SchemaEditor>
            </Flex>
          </Col>
          <Col span={8} offset={8}>
            <Flex justify="flex-end" gap="small">
              <Button type="default" size="middle">
                Default
              </Button>
              <Button type="primary" htmlType="submit" size="middle">
                Submit
              </Button>
            </Flex>
          </Col>
        </Row>
      </Form>
    </>
  );
};
