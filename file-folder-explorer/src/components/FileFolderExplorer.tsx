import React, {useEffect, useState} from 'react';
import {createFolder, getAllFolders, getFolderById, uploadFile} from "../apiService";
import {parseFolders} from '../utils';

interface Folder {
    folderId: string;
    parentFolderId: string;
    name: string;
    subFolders: Folder[];
    files: File[];
    parentFolder: Folder | null;
}

interface File {
    fileId: string;
    name: string;
}

const FileFolderExplorer = () => {
    const [currentFolder, setCurrentFolder] = useState<Folder | null>(null);
    const [currentFolderId, setCurrentFolderId] = useState<string>('');
    const [newFolderName, setNewFolderName] = useState<string>('');
    const [breadcrumb, setBreadcrumb] = useState<Folder[]>([]);

    useEffect(() => {
        const fetchFolders = async () => {
            if (!currentFolderId) {
                const folders = await getAllFolders();
                const parsedFolders = parseFolders(folders);
                if (parsedFolders.length > 0) {
                    setCurrentFolder(parsedFolders[0]);
                    updateBreadcrumb(parsedFolders[0]);
                }
            } else {
                const folder = await getFolderById(currentFolderId);
                setCurrentFolder(folder);
                updateBreadcrumb(folder);
            }
        };
        fetchFolders();
    }, [currentFolderId]);

    const updateBreadcrumb = (folder: Folder) => {
        const newBreadcrumb = [];
        let current: Folder | null = folder;
        while (current) {
            newBreadcrumb.unshift(current);
            current = current.parentFolder;
        }
        setBreadcrumb(newBreadcrumb);
    };

    const handleCreateFolder = async () => {
        if (newFolderName) {
            await createFolder(newFolderName, currentFolderId);
            setNewFolderName('');
            const folder = await getFolderById(currentFolderId);
            setCurrentFolder(folder);
        }
    };

    const handleUploadFile = async (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event?.target?.files?.[0] && currentFolderId) {
            await uploadFile(event?.target?.files?.[0], currentFolderId);
            const folder = await getFolderById(currentFolderId);
            setCurrentFolder(folder);
        }
    };

    return (
        <div>
            <h1>Welcome to Folder Explorer</h1>
            <div>
                <input
                    type="text"
                    value={newFolderName}
                    onChange={(event) => setNewFolderName(event.target.value)}
                    placeholder={'Enter folder name'}
                />
                <button onClick={handleCreateFolder}>Create Folder</button>
            </div>
            <div>
                <input type="file" accept=".csv,.geojson" onChange={handleUploadFile}/>
                <button>Upload file</button>
            </div>
            <div>
                <div>
                    {breadcrumb.map((folder, index) => (
                        <span key={folder.folderId}>
                            {index > 0 && ' / '}
                            <span onClick={() => setCurrentFolderId(folder.folderId)}>
                                {folder.name}
                            </span>
                        </span>
                    ))}
                </div>
            </div>
            {currentFolder && (
                <div>
                    <h2>{currentFolder.name}</h2>
                    <div style={{display: 'flex'}}>
                        <div style={{flex: 1}}>
                            <ul>
                                {currentFolder.parentFolderId ? (
                                    <li onClick={() => setCurrentFolderId('')}>Back to root</li>
                                ) : null}
                                {currentFolder.subFolders.map((folder) => (
                                    <li key={folder.folderId} onClick={() => setCurrentFolderId(folder.folderId)}>
                                        {folder.name}
                                    </li>
                                ))}
                            </ul>
                        </div>
                        <div style={{flex: 3}}>
                            <h3>Content</h3>
                            <ul>
                                {currentFolder.subFolders.map((folder) => (
                                    <li key={folder.folderId} onClick={() => setCurrentFolderId(folder.folderId)}>
                                        📁 {folder.name}
                                    </li>
                                ))}
                                {Array.isArray(currentFolder.files) && currentFolder.files.map((file) => (
                                    <li key={file.fileId}>
                                        📄 {file.name}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default FileFolderExplorer;
