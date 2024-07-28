import React, {useEffect, useState} from 'react';
import {Line} from 'react-chartjs-2';
import * as d3 from 'd3';
import {
    CategoryScale,
    Chart as ChartJS,
    Legend,
    LinearScale,
    LineElement,
    PointElement,
    Title,
    Tooltip
} from 'chart.js';

// Register Chart.js components
ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend);

interface CsvContentProps {
    data: string;
}

function CsvContent({data}: CsvContentProps) {
    const [chartData, setChartData] = useState<any>(null);

    useEffect(() => {
        if (!data) {
            return;
        }

        // Have to reformat the string to CSV in order to parse it
        const lines = data.trim().split('\n');
        if (lines.length === 0) {
            return;
        }
        const csvString = lines.join('\n');
        const parsedData = d3.csvParse(csvString);

        // Ensure parsed data is valid
        if (parsedData.length === 0) {
            return;
        }

        // create the chart data
        const labels = Object.keys(parsedData[0]);
        const chartLabels = parsedData.map((d: any) => d[labels[0]]);
        const datasets = labels.slice(1).map((label, index) => ({
            label,
            data: parsedData.map((d: any) => parseFloat(d[label])),
            fill: false,
            backgroundColor: `rgba(4, 59, 92)`,
            borderColor: `rgba(192, 192, 1)`,
        }));

        setChartData({
            labels: chartLabels,
            datasets,
        });
    }, [data]);

    // bit of a hacky way to handle the case where the data is not valid
    if (!chartData) {
        return <div></div>;
    }

    return (
        <div>
            <Line data={chartData}/>
        </div>
    );
}

export default CsvContent;
