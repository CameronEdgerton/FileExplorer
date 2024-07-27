import React from 'react';
import {Folder} from "../interfaces";

interface FolderStructureProps {
    data: Folder | null;
    onFolderClick: (folderId: string) => void;
}

function FolderStructure({data, onFolderClick}: FolderStructureProps) {
    /*const [expanded, setExpanded] = React.useState(false);*/

    if (!data) {
        return null;
    }

    const handleFolderClick = (folderId: string) => {
        onFolderClick(folderId);
    };

    return (
        <div style={{marginTop: 5}}>
            <div>
                <span onClick={() => handleFolderClick(data.folderId)}>📁 {data.name}</span>
                {/*<button onClick={() => setExpanded(!expanded)}>{expanded ? '🔽' : '▶️'}</button>*/}
            </div>
            <div style={{marginLeft: 20}}>
                {data.subFolders && data.subFolders.map((item) => (
                    <FolderStructure key={item.folderId} data={item} onFolderClick={onFolderClick}/>
                ))}
            </div>
        </div>
    );
}

export default FolderStructure;