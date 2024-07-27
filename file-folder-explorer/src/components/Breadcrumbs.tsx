import React from 'react';
import {Folder} from "../interfaces";

interface BreadcrumbsProps {
    data: Folder | null;
    onBreadcrumbClick: (folderId: string) => void;
}

function Breadcrumbs({data, onBreadcrumbClick}: BreadcrumbsProps) {

    if (!data) {
        return null;
    }

    const handleBreadcrumbClick = (folderId: string) => {
        onBreadcrumbClick(folderId);
    };

    const buildBreadcrumbTrail = (folder: Folder | null): React.ReactNode[] => {
        const trail: React.ReactNode[] = [];

        const traverseUp = (current: Folder | null) => {
            if (current?.parentFolder) {
                traverseUp(current.parentFolder);
            }
            if (current) {
                trail.push(
                    <span key={current.folderId} onClick={() => handleBreadcrumbClick(current.folderId)}>
                        {current.name} {current.parentFolderId !== null && " / "}
                    </span>
                );
            }
        };

        traverseUp(folder);
        return trail;
    };

    return <div>{buildBreadcrumbTrail(data)}</div>;
}

export default Breadcrumbs;