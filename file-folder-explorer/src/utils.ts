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

export const parseFolders = (data: any): Folder[] => {
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
            }))
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

    return Array.isArray(data?.$values) ? data.$values.map((folderData: any) => createFolder(folderData)) : [];
};
