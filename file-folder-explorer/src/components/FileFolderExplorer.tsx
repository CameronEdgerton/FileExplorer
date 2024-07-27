import React, {useEffect, useRef, useState} from 'react';
import {createFolder, getAllFolders, getFolderById, uploadFile} from "../apiService";
import {parseFolders, parseSingleFolder} from '../utils';
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
        const fetchAllFolders = async () => {
            const folders = await getAllFolders();
            const parsedFolders = parseFolders(folders);
            if (parsedFolders.length > 0) {
                setRootFolder(parsedFolders[0]);
                updateBreadcrumb(parsedFolders[0]);
            }
        };
        fetchAllFolders();
    }, [folderAdded]);

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
            setFolderAdded(!folderAdded);
            /*            const folder = await getFolderById(currentFolderId);
                        setCurrentFolder(folder);
                        updateBreadcrumb(folder);*/
        }
    };

    const handleUploadFile = async (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event?.target?.files?.[0] && currentFolderId) {
            console.log('Selected file:', event.target.files[0]); // Log the selected file
            await uploadFile(event.target.files[0], currentFolderId);
            /*const folder = await getFolderById(currentFolderId);
            setCurrentFolder(folder);*/
        } else {
            console.log('No file selected or currentFolderId is missing');
        }
    };

    const handleFolderClick = async (folderId: string) => {
        setCurrentFolderId(folderId);
        const folder = await getFolderById(folderId);
        const parsedFolder = parseSingleFolder(folder);
        setCurrentFolder(parsedFolder);
        console.log('Current folder:', parsedFolder);
        updateBreadcrumb(parsedFolder);
    };


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
};

export default FileFolderExplorer;