import React, {useEffect, useRef, useState} from 'react';
import {createFolder, getFolderById, getFolderTree, uploadFile} from "../apiService";
import {findPathToFolder, parseSingleFolder} from '../utils';
import {Folder} from "../interfaces";
import FolderStructure from "./FolderStructure";
import FolderContents from "./FolderContents";

const FileFolderExplorer = () => {
    const [currentFolder, setCurrentFolder] = useState<Folder | null>(null);
    const [rootFolder, setRootFolder] = useState<Folder | null>(null);
    const [currentFolderId, setCurrentFolderId] = useState<string>('');
    const [newFolderName, setNewFolderName] = useState<string>('');
    const [folderAdded, setFolderAdded] = useState<boolean>(false);
    const [breadcrumb, setBreadcrumb] = useState<Folder[]>([]);
    const fileInputRef = useRef<HTMLInputElement>(null);


    useEffect(() => {
        const fetchFolderTree = async () => {
            const rootFolder = await getFolderTree();
            if (!rootFolder) {
                return;
            }
            const parsedFolder = parseSingleFolder(rootFolder);
            if (parsedFolder) {
                setRootFolder(parsedFolder);
            }
        };
        fetchFolderTree();
    }, [folderAdded]);


    const handleCreateFolder = async () => {
        if (newFolderName) {
            await createFolder(newFolderName, currentFolderId);
            setNewFolderName('');
            setFolderAdded(!folderAdded);
        }
    };

    const handleUploadFile = async (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event?.target?.files?.[0] && currentFolderId) {
            await uploadFile(event.target.files[0], currentFolderId);
            if (currentFolder?.folderId == currentFolderId) {
                const folder = await getFolderById(currentFolderId);
                const parsedFolder = parseSingleFolder(folder);
                setCurrentFolder(parsedFolder);
            } else {
                console.log('No file selected or currentFolderId is missing');
            }
        }
    };

    const handleFolderClick = async (folderId: string) => {
        setCurrentFolderId(folderId);
        const folder = await getFolderById(folderId);
        const parsedFolder = parseSingleFolder(folder);
        setCurrentFolder(parsedFolder);

        const breadcrumbTrail = findPathToFolder(rootFolder, folderId);
        setBreadcrumb(breadcrumbTrail);
    };

// Function to set the initial breadcrumb based on the root folder
    const initializeBreadcrumb = (root: Folder) => {
        const initialBreadcrumb = [root];
        setBreadcrumb(initialBreadcrumb);
    };

// Call initializeBreadcrumb when you set the root folder
    useEffect(() => {
        if (rootFolder) {
            initializeBreadcrumb(rootFolder);
        }
    }, [rootFolder]);


    const triggerFileInput = () => {
        fileInputRef.current?.click();
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
            <div style={{display: 'flex'}}>
                {rootFolder && (
                    <FolderStructure data={rootFolder} onFolderClick={handleFolderClick}/>
                )}

                {currentFolder && (
                    <div>
                        <div>
                            <div>
                                {breadcrumb.map((folder, index) => (
                                    <span key={folder.folderId}>
                                    {index > 0 && ' / '}
                                        <span onClick={() => handleFolderClick(folder.folderId)}>
                                            {folder.name}
                                        </span>
                                    </span>
                                ))}
                            </div>
                        </div>
                        <div style={{display: 'flex'}}>
                            <div style={{flex: 3}}>
                                <FolderContents data={currentFolder}/>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default FileFolderExplorer;