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
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [error, setError] = useState<string | null>(null);


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
        <div className=' m-6'>
            <h1 className='text-2xl w-full text-center font-bold'>Welcome to Folder Explorer</h1>

            <div className="flex w-min-full mt-8">
                <div className=' flex justify-center items-center border-r-[1px] border-solid border-slate-300 px-6'>
                    <input
                        type="file"
                        accept=".csv,.geojson"
                        style={{display: 'none'}}
                        ref={fileInputRef}
                        onChange={handleUploadFile}
                    />
                    <button
                        className="text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:ring-blue-300 font-medium 
                        rounded-lg text-sm px-5 py-2.5 focus:outline-none"
                        onClick={triggerFileInput}>Upload File
                    </button>
                </div>

                <div className=' flex gap-4 justify-center items-center px-6 w-[50vw]'>
                    <button
                        className="w-1/3 text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:ring-blue-300 
                        font-medium rounded-lg text-sm px-5 py-2.5 focus:outline-none"
                        onClick={handleCreateFolder}>Create Folder
                    </button>
                    <input
                        className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg 
                        focus:ring-blue-500 focus:border-blue-500 block w-2/3 p-2.5"
                        type="text"
                        value={newFolderName}
                        onChange={(event) => setNewFolderName(event.target.value)}
                        placeholder={'Folder name to be created'}
                    />
                </div>
            </div>

            {error && <div className='text-red-700 font-medium w-fit px-4 bg-red-100 ml-6 mt-4 py-[0.5]'>{error}</div>}

            <div className="flex gap-20 mt-8">
                <div className='border-r-[1.5px] border-solid border-slate-300 pl-6 pr-40'>
                    {folderTree && (
                        <FolderStructure data={folderTree} onFolderClick={handleFolderClick}/>
                    )}
                </div>

                <div className='flex flex-col gap-16'>
                    {currentFolder && (
                        <>
                            <div>
                                <div>
                                    <Breadcrumbs data={breadcrumbs} onBreadcrumbClick={handleFolderClick}/>
                                </div>
                                <div className='flex gap-1 flex-col'>
                                    <FolderContents data={currentFolder}/>
                                </div>
                            </div>
                            <div>
                                <p className='text-lg font-medium'>File Preview : </p>
                                <div className=' w-[30rem] h-60 bg-blue-200 flex justify-center items-center rounded'>
                                    TO DO
                                </div>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}

export default FileFolderExplorer;