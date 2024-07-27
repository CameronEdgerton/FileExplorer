import {Folder} from "../interfaces";
import {useState} from "react";

interface FolderStructureProps {
    data: Folder;
    onFolderClick: (folderId: string) => void;
}

function FolderStructure({data, onFolderClick}: FolderStructureProps) {

    const [expanded, setExpanded] = useState<boolean>(false);
    const handleFolderClick = (folderId: string) => {
        onFolderClick(folderId);
    };

    return (
        <div className=' my-2'>
            <div className="relative flex py-2 items-center">
                {<button onClick={() => setExpanded(!expanded)}>{expanded ? '🔽' : '▶️'}</button>}
                <span className='font-medium cursor-pointer'
                      onClick={() => handleFolderClick(data.folderId)}> 📁 {data.name}</span>
            </div>

            <div className='ml-8 pl-2' style={{display: expanded ? "block" : "none"}}>
                {data.subFolders && data.subFolders.map((item) => (
                    <FolderStructure key={item.folderId} data={item} onFolderClick={onFolderClick}/>
                ))}
            </div>
        </div>
    );
}

export default FolderStructure;