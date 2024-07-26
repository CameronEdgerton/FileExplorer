import React, {useEffect, useRef, useState} from 'react';
import {createFolder, getAllFolders, getFolderById, uploadFile} from "../apiService";
import {parseFolders} from '../utils';
import {Folder} from "../interfaces";
import FolderStructure from "./FolderStructure";


const FileFolderExplorer = () => {
    const [currentFolder, setCurrentFolder] = useState<Folder | null>(null);
    const [currentFolderId, setCurrentFolderId] = useState<string>('');
    const [newFolderName, setNewFolderName] = useState<string>('');
    const [breadcrumb, setBreadcrumb] = useState<Folder[]>([]);
    const fileInputRef = useRef<HTMLInputElement>(null);


    /*useEffect(() => {
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
    }, [currentFolderId]);*/

    useEffect(() => {
        const fetchAllFolders = async () => {
            const folders = await getAllFolders();
            const parsedFolders = parseFolders(folders);
            if (parsedFolders.length > 0) {
                setCurrentFolder(parsedFolders[0]);
                updateBreadcrumb(parsedFolders[0]);
            }
        };
        fetchAllFolders();
    }, [currentFolderId]);

    const updateBreadcrumb = (folder: Folder) => {
        const newBreadcrumb: Folder[] = [];
        const buildBreadcrumb = (folder: Folder) => {
            if (folder.parentFolder) {
                buildBreadcrumb(folder.parentFolder);
            }
            newBreadcrumb.push(folder);
        };
        buildBreadcrumb(folder);
        setBreadcrumb(newBreadcrumb);
    };

    const handleCreateFolder = async () => {
        if (newFolderName) {
            await createFolder(newFolderName, currentFolderId);
            setNewFolderName('');
            const folder = await getFolderById(currentFolderId);
            setCurrentFolder(folder);
            updateBreadcrumb(folder);
        }
    };

    const handleUploadFile = async (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event?.target?.files?.[0] && currentFolderId) {
            console.log('Selected file:', event.target.files[0]); // Log the selected file
            await uploadFile(event.target.files[0], currentFolderId);
            const folder = await getFolderById(currentFolderId);
            setCurrentFolder(folder);
        } else {
            console.log('No file selected or currentFolderId is missing');
        }
    };

    const handleFolderClick = async (folderId: string) => {
        setCurrentFolderId(folderId);
        const folder = await getFolderById(folderId);
        setCurrentFolder(folder);
        console.log('Folder clicked:', folderId);
    };


    const triggerFileInput = () => {
        fileInputRef.current?.click();
    };

    const renderSubFolders = (folder: Folder) => {
        return (
            <ul>
                {/* Render the current folder itself */}
                <li key={folder.folderId}
                    onClick={() => {
                        console.log('Folder clicked:', folder.folderId);
                        setCurrentFolderId(folder.folderId);
                    }}>
                    {folder.name}
                    {/* Render subfolders recursively */}
                    {folder.subFolders && folder.subFolders.length > 0 && (
                        <ul>
                            {folder.subFolders.map((subFolder) => renderSubFolders(subFolder))}
                        </ul>
                    )}
                </li>
            </ul>
        );
    };

    return (
        <div>
            <h1>Welcome to Folder Explorer</h1>
            <div style={{display: 'flex'}}>
                <div>
                    <input
                        type="file"
                        accept=".csv,.geojson"
                        style={{display: 'none'}}
                        ref={fileInputRef}
                        onChange={handleUploadFile}
                    />
                    <button onClick={triggerFileInput}>Upload File</button>
                </div>
                <div>
                    <button onClick={handleCreateFolder}>Create Folder</button>
                    <input
                        type="text"
                        value={newFolderName}
                        onChange={(event) => setNewFolderName(event.target.value)}
                        placeholder={'Folder name to be created'}
                    />
                </div>
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
                    <div style={{display: 'flex'}}>
                        <FolderStructure data={currentFolder} onFolderClick={handleFolderClick}/>
                        <div style={{flex: 3}}>
                            <h3>{currentFolder.name}</h3>
                            <ul>
                                {currentFolder.subFolders && currentFolder.subFolders.map((folder) => (
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