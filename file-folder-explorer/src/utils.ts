﻿import {File, Folder} from './interfaces';

export const parseSingleFolder = (folderData: any): Folder => {
    const refMap = new Map<string, Folder>();

    const createFolder = (folderData: any): Folder => {
        if (refMap.has(folderData.$id)) {
            return refMap.get(folderData.$id)!;
        }

        const folder: Folder = {
            folderId: folderData.folderId,
            parentFolderId: folderData.parentFolderId,
            name: folderData.name,
            subFolders: [],
            files: [],
            parentFolder: null,
        };

        refMap.set(folderData.$id, folder);

        folder.files = Array.isArray(folderData.files?.$values)
            ? folderData.files.$values.map((file: any) => ({
                fileId: file.fileId,
                name: file.name
            }) as File)
            : [];

        folder.subFolders = Array.isArray(folderData.subfolders?.$values)
            ? folderData.subfolders.$values.map((subfolderData: any) => {
                const subfolder = createFolder(subfolderData);
                subfolder.parentFolder = folder;
                return subfolder;
            })
            : [];

        return folder;
    };

    return createFolder(folderData);
};
