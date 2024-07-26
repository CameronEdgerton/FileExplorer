import React from 'react';
import {Route, Routes} from 'react-router-dom';
import FileFolderExplorer from "./components/FileFolderExplorer";

function App() {
    /*const [currentFolder, setCurrentFolder] = useState<Folder | null>(null);

    useEffect(() => {
        const fetchAllFolders = async () => {
            const folders = await getAllFolders();
            const parsedFolders = parseFolders(folders);
            if (parsedFolders.length > 0) {
                setCurrentFolder(parsedFolders[0]);
            }
        };
        fetchAllFolders();
    }, []);*/


    return (
        <Routes>
            <Route path='/' element={<FileFolderExplorer/>}/>
        </Routes>
    );
}

export default App;
