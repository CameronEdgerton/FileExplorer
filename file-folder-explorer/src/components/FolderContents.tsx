import {Folder} from "../interfaces";

interface FolderContentProps {
    data: Folder
}

function FolderContents({data}: FolderContentProps) {

    return (
        <div className=" my-2 flex flex-col">
            <div>
                <span>...</span>
            </div>
            <div className=" gap-1 flex flex-col">
                {data.subFolders && data.subFolders.map((item) => (
                    <span key={item.folderId}>📁 {item.name}</span>
                ))}
            </div>
            <div className=" gap-1 flex flex-col">
                {data.files && data.files.map((item) => (
                    <span key={item.fileId}>📄 {item.name}</span>
                ))}
            </div>
        </div>
    );
}

export default FolderContents;