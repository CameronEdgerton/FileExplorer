import React from 'react';
import {Folder} from "../interfaces";

interface FolderContentProps {
    data: Folder | null;
}

function FolderContents({data}: FolderContentProps) {

    if (!data) {
        return null;
    }

    return (
        <div style={{marginTop: 5}}>
            <div>
                <span>...</span>
            </div>
            <div style={{marginLeft: 20}}>
                {data.subFolders && data.subFolders.map((item) => (
                    <span key={item.folderId}>📁 {item.name}</span>
                ))}
            </div>
            <div style={{marginLeft: 20}}>
                {Array.isArray(data.files) && data.files.map((item) => (
                    <span key={item.fileId}>📄 {item.name}</span>
                ))}
            </div>
        </div>
    );
}

export default FolderContents;