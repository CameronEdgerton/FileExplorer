import {Folder} from "../interfaces";

interface BreadcrumbsProps {
    data: Folder[];
    onBreadcrumbClick: (folderId: string) => void;
}

function Breadcrumbs({data, onBreadcrumbClick}: BreadcrumbsProps) {

    if (!data) return null;

    const handleBreadcrumbClick = (folderId: string) => {
        onBreadcrumbClick(folderId);
    };

    return (
        <div>
            {data.map((folder, index) => (
                <span key={folder.folderId} className="font-medium">
                    {index > 0 && ' / '}
                    <span className="underline cursor-pointer"
                          onClick={() => handleBreadcrumbClick(folder.folderId)}>{folder.name}</span>
                </span>
            ))}
        </div>
    )
}

export default Breadcrumbs;