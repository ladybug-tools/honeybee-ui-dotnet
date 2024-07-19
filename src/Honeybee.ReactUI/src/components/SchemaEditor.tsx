import React, { useState } from 'react';
import { Button, Modal } from 'antd';
import TextArea from 'antd/es/input/TextArea';
import { EfficiencyStandards, ProjectInfo } from 'honeybee-schema-sdk';

// type StringReturningFunction = () => string;

export const SchemaEditor: React.FC<{getData:() => Promise<string>, onClose:(schema:string)=>void}> = ({getData, onClose}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [schemaJson, setSchemaJson] = useState("");

  const showModal = () => {
    getData().then((d)=> {
        setSchemaJson(d);
        setIsModalOpen(true);
    });

  };


  function isValidMyClass(obj: any): obj is ProjectInfo {
    const enumValues = Object.values(EfficiencyStandards);
    const v: any[]= obj.vintage;
    const done = v.every(value => enumValues.includes(value));
    return done;
  }

  const handleOk = () => {
    const jObj = JSON.parse(schemaJson);
    const hbObj = ProjectInfo.fromJS(jObj);
    var isValid = isValidMyClass(hbObj);
    if (!isValid) {
      console.log(hbObj);
      return;
    }
    
    setIsModalOpen(false);
    onClose(schemaJson);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const handleChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
    setSchemaJson(e.target.value);
  };

  return (
    <>
      <Button type="default" onClick={showModal} size="middle">
        Data
      </Button>
      <Modal title="Basic Modal" open={isModalOpen} onOk={handleOk} onCancel={handleCancel}>
      <TextArea
        autoSize={{ minRows: 20, maxRows: 20 }}
        style={{ whiteSpace: 'pre', overflowX: 'scroll' }}
        value={schemaJson}
        onChange={(e) => setSchemaJson(e.target.value)}
      />
      </Modal>
    </>
  );
};
