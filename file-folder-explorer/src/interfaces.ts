export interface Folder {
    folderId: string;
    parentFolderId: string | null;
    name: string;
    subFolders: Folder[];
    files: File[];
    parentFolder: Folder | null;
}

export interface File {
    fileId: string;
    name: string;
}