import React from 'react';
import {Route, Routes} from 'react-router-dom';
import FileFolderExplorer from './components/FileFolderExplorer';

function App() {
    return (
        <Routes>
            <Route path='/' element={<FileFolderExplorer/>}/>
        </Routes>
    );
}

export default App;
