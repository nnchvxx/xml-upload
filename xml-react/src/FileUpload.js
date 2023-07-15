import React, { useState, useEffect } from "react";
import axios from "axios";
import { Form, Table } from "react-bootstrap";
import Button from "react-bootstrap/Button";

const FileUpload = () => {
  const [file, setFile] = useState(null);
  const [uploadStatus, setUploadStatus] = useState("");
  const [inputKey, setInputKey] = useState(Date.now());
  const [fileStatuses, setFileStatuses] = useState([]);

  const handleFileChange = (event) => {
    setFile(event.target.files[0]);
  };

  const handleSubmit = (event) => {
    event.preventDefault();

    if (!file) {
      setUploadStatus("Please select a file.");
      return;
    }

    const formData = new FormData();
    formData.append("file", file);

    axios
      .post("https://localhost:7097/fileconvert", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => {
        setFileStatuses((prevFileStatuses) => [
          ...prevFileStatuses,
          { fileId: response.data, status: "Processing" },
        ]);
        setUploadStatus("");
        setInputKey(Date.now());
      })
      .catch((error) => {
        setUploadStatus(`Error uploading file: ${error.message}`);
      });

    setFile(null);
  };

  useEffect(() => {
    const timer = setInterval(() => {
      fileStatuses.forEach((fileStatus, index) => {
        axios
          .get(`https://localhost:7097/fileconvert/${fileStatus.fileId}`)
          .then((response) => {
            const newFileStatuses = [...fileStatuses];
            newFileStatuses[index].status = response.data.status;
            setFileStatuses(newFileStatuses);
          });
      });
    }, 5000);

    return () => {
      clearInterval(timer);
    };
  }, [fileStatuses]);

  return (
    <div
      className="d-flex justify-content-center align-items-center"
      style={{ height: "100vh", backgroundColor: "#f5f5f5" }}
    >
      <div
        style={{
          display: "flex",
          flexDirection: "row",
          justifyContent: "space-between",
          alignItems: "flex-start",
          width: "80%",
          height: "80%",
          padding: "20px",
          borderRadius: "5px",
          boxShadow: "0px 0px 10px rgba(0,0,0,0.1)",
          backgroundColor: "#fff",
        }}
      >
        <div style={{ marginRight: "20px", width: "30%" }}>
          <h2 style={{ marginBottom: "20px" }}>Upload XML File</h2>
          <Form onSubmit={handleSubmit}>
            <Form.Group>
              <Form.Control
                type="file"
                onChange={handleFileChange}
                key={inputKey}
              />
            </Form.Group>
            <Button type="submit" variant="primary" block className="mt-2">
              Upload
            </Button>
            {uploadStatus && (
              <div
                style={{
                  marginTop: "20px",
                  padding: "10px",
                  borderRadius: "5px",
                  backgroundColor: uploadStatus.startsWith("Error")
                    ? "#f8d7da"
                    : "#d4edda",
                  color: uploadStatus.startsWith("Error")
                    ? "#721c24"
                    : "#155724",
                }}
              >
                {uploadStatus}
              </div>
            )}
          </Form>
        </div>

        <div
          style={{
            overflowY: "auto",
            height: "100%",
            width: "60%",
            borderLeft: "1px solid #ddd",
            paddingLeft: "20px",
          }}
        >
          {fileStatuses.length > 0 && (
            <>
              <h5>File Statuses</h5>
              <Table striped bordered hover size="sm">
                <thead>
                  <tr>
                    <th>File ID</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {fileStatuses.map((fileStatus) => (
                    <tr key={fileStatus.fileId}>
                      <td>{fileStatus.fileId}</td>
                      <td>{fileStatus.status}</td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default FileUpload;
