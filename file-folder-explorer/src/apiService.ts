﻿import axios from "axios";

const API_URL = 'http://localhost:5054/api';


export const getFolderTree = async () => {
    const url = `${API_URL}/folders/tree`;
    const response = await axios.get(url);
    return response.data;
};

export const getFolderById = async (folderId: string) => {
    const url = `${API_URL}/folders/${folderId}`;

    const response = await axios.get(url);
    return response.data;
};

export const createFolder = async (folderName: string, parentFolderId: string) => {
    const url = `${API_URL}/folders/create`;

    const response = await axios.post(url, {
        folderName,
        parentFolderId
    });
    return response.data;
};

export const uploadFile = async (file: File, folderId: string) => {
    const url = `${API_URL}/files/upload`;

    const formData = new FormData();
    formData.append('file', file);
    formData.append('folderId', folderId);

    const response = await axios.post(url, formData);
    return response.data;
};

export const getFileContent = async (fileId: string) => {
    const url = `${API_URL}/files/${fileId}`;

    const response = await axios.get(url, {responseType: 'text'});
    return response.data;
}
