import React, {useEffect, useRef, useState} from 'react';
import {createFolder, getFolderById, getFolderTree, uploadFile} from "../apiService";
import {findPathToFolder, parseSingleFolder} from '../utils';
import {Folder} from "../interfaces";
import FolderStructure from "./FolderStructure";
import FolderContents from "./FolderContents";
import Breadcrumbs from "./Breadcrumbs";

const FileFolderExplorer = () => {
    const [folderTree, setFolderTree] = useState<Folder | null>(null);
    const [currentFolder, setCurrentFolder] = useState<Folder | null>(null);
    const [currentFolderId, setCurrentFolderId] = useState<string>('');
    const [newFolderName, setNewFolderName] = useState<string>('');
    const [folderAdded, setFolderAdded] = useState<boolean>(false);
    const [breadcrumbs, setBreadcrumbs] = useState<Folder[]>([]);
    const [error, setError] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);


    useEffect(() => {
        const fetchFolderTree = async () => {
            try {
                const rootFolder = await getFolderTree();
                if (!rootFolder) {
                    return;
                }
                const parsedFolder = parseSingleFolder(rootFolder);
                if (parsedFolder) {
                    setFolderTree(parsedFolder);
                }
                setError(null);
            } catch (error: unknown) {
                setError('Error fetching folder tree');
            }
        };
        fetchFolderTree();
    }, [folderAdded]);


    const handleCreateFolder = async () => {
        if (newFolderName) {
            try {
                await createFolder(newFolderName, currentFolderId);
                setError(null);
            } catch (error: unknown) {
                setError('Error creating folder');
            }
            setNewFolderName('');
            setFolderAdded(!folderAdded);
        }
    };

    const handleUploadFile = async (event: React.ChangeEvent<HTMLInputElement>) => {
        if (currentFolderId === '') {
            setError('You must select a folder to upload the file to');
            return;
        } 
        setError(null);
        
        if (event?.target?.files?.[0] && currentFolderId) {
            try {
                await uploadFile(event.target.files[0], currentFolderId);
                setError(null);
            } catch (error: unknown) {
                setError('Error uploading file');
            }

            // if we have the current folder open, we need to refresh the contents
            if (currentFolder?.folderId == currentFolderId) {
                const folder = await getFolderById(currentFolderId);
                const parsedFolder = parseSingleFolder(folder);
                setCurrentFolder(parsedFolder);
            }
        }
    };

    const handleFolderClick = async (folderId: string) => {
        setCurrentFolderId(folderId);
        try {
            const folder = await getFolderById(folderId);
            const parsedFolder = parseSingleFolder(folder);
            setCurrentFolder(parsedFolder);
            setError(null);
        } catch (error: unknown) {
            setError('Error fetching folder');
        }

        const breadcrumbTrail = findPathToFolder(folderTree, folderId);
        setBreadcrumbs(breadcrumbTrail);
    };

    // Call initializeBreadcrumb when you set the root folder
    useEffect(() => {
        if (folderTree) {
            initializeBreadcrumb(folderTree);
        }
    }, [folderTree]);

    // Function to set the initial breadcrumb based on the root folder
    const initializeBreadcrumb = (root: Folder) => {
        const initialBreadcrumb = [root];
        setBreadcrumbs(initialBreadcrumb);
    };

    const triggerFileInput = () => {
        fileInputRef.current?.click();
    };

    return (
        <div>
            <h1>Welcome to Folder Explorer</h1>
            {error && <div style={{color: 'red'}}>{error}</div>}
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
                {folderTree && (
                    <FolderStructure data={folderTree} onFolderClick={handleFolderClick}/>
                )}

                {currentFolder && (
                    <div>
                        <div>
                            <Breadcrumbs data={breadcrumbs} onBreadcrumbClick={handleFolderClick}/>
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