import React, { useState, useEffect } from "react";
import axios from "axios";
import { Form, Table } from "react-bootstrap";
import Button from "react-bootstrap/Button";

const FileUpload = () => {
  const [files, setFiles] = useState([]);
  const [uploadStatus, setUploadStatus] = useState("");
  const [inputKey, setInputKey] = useState(Date.now());
  const [fileStatuses, setFileStatuses] = useState([]);
  const [fileNames, setFileNames] = useState([]);

  const listFileNames = () => {
    axios
      .get("https://localhost:7097/fileconvert/list")
      .then((response) => {
        var result = response.data;
        setFileNames(result);
      })
      .catch((error) => {});
  };

  useEffect(() => {
    listFileNames();
  }, [fileStatuses]);

  const handleFileDownload = async (fileName) => {
    try {
      const response = await axios.get(
        `https://localhost:7097/fileconvert/download/${fileName}`,
        {
          responseType: "blob",
        }
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
    } catch (error) {
      console.error("Error downloading file:", error);
    }
  };

  const handleFileChange = (event) => {
    setFiles(event.target.files);
  };

  const handleSubmit = (event) => {
    event.preventDefault();

    if (!files.length) {
      setUploadStatus("Please select a file.");
      return;
    }

    const formData = new FormData();
    Array.from(files).forEach((file) => {
      formData.append("files", file);
    });

    axios
      .post("https://localhost:7097/fileconvert", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((response) => {
        setFileStatuses((prevFileStatuses) => [
          ...prevFileStatuses,
          ...response.data.map((fileId) => ({ fileId, status: "Processing" })),
        ]);
        setUploadStatus("");
        setInputKey(Date.now());
      })
      .catch((error) => {
        setUploadStatus(`Error uploading file: ${error.message}`);
      });

    setFiles([]);
  };

  useEffect(() => {
    const timer = setInterval(() => {
      if (fileStatuses.length > 0) {
        const fileIds = fileStatuses.map((fileStatus) => fileStatus.fileId);

        axios
          .post(`https://localhost:7097/fileconvert/status`, fileIds)
          .then((response) => {
            const updatedStatuses = response.data;
            const newFileStatuses = fileStatuses.map((fileStatus) => {
              const updatedStatus = updatedStatuses.find(
                (status) => status.fileId === fileStatus.fileId
              );
              return updatedStatus ? updatedStatus : fileStatus;
            });

            setFileStatuses(newFileStatuses);
          });
      }
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
                multiple
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
          <div>
            {fileNames.map((fileName) => {
              return (
                <div>
                  <button onClick={() => handleFileDownload(fileName)}>
                    {fileName}
                  </button>
                  <br></br>
                </div>
              );
            })}
          </div>
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
