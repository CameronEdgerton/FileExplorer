import React, {useRef} from 'react';

interface UploadFileButtonProps {
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

function UploadFileButton({onChange}: UploadFileButtonProps) {
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleUploadFile = (event: React.ChangeEvent<HTMLInputElement>) => {
        onChange(event);
    };

    const triggerFileInput = () => {
        fileInputRef.current?.click();
    };

    return (
        <div>
            <input
                type="file"
                accept=".csv,.geojson"
                style={{display: 'none'}}
                ref={fileInputRef}
                onChange={handleUploadFile}
            />
            <button onClick={triggerFileInput}>Upload File</button>
        </div>
    );
}

export default UploadFileButton;
