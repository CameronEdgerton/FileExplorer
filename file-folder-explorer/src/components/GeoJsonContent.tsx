import L from 'leaflet';
import {useEffect} from "react";

interface GeoJsonContentProps {
    data: any;
}

function GeoJsonContent({data}: GeoJsonContentProps) {
    useEffect(() => {
        if (!data) {
            return;
        }
        // Initialize the map
        const map = L.map('map').setView([0, 0], 2);

        // Set up the tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
        }).addTo(map);

        // Add the GeoJSON data to the map
        L.geoJSON(data).addTo(map);

        // Fit map bounds to the GeoJSON layer
        map.fitBounds(L.geoJSON(data).getBounds());

        // Cleanup function
        return () => {
            map.remove();
        };
    }, [data]);

    return <div id="map" style={{height: '500px', width: '100%'}}/>;
}

export default GeoJsonContent;