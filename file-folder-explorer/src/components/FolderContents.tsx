import {File, Folder} from "../interfaces";

interface FolderContentProps {
    data: Folder,
    onFileClick: (file: File) => void;
}

function FolderContents({data, onFileClick}: FolderContentProps) {

    const handleFileClick = (file: File) => {
        onFileClick(file);
    };

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
                    <span className=" cursor-pointer" key={item.fileId}
                          onClick={() => handleFileClick(item)}>📄 {item.name}</span>
                ))}
            </div>
        </div>
    );
}

export default FolderContents;