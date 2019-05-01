$(document).ready(function () {
    var categories = ['15-20', '21-25', '26-30', '31-35', '36-40', '41-45', '46-50', '51-55', '56-60', '60 + '];

    var chart = new Highcharts.Chart({
        exporting: {
            buttons: {
                contextButton: {
                    enabled: false
                }
            }
        },
        chart: {
            renderTo: 'hospitalization-risk',
            type: 'column',
            options3d: {
                enabled: true,
                alpha: 12,
                beta: 19,
                depth: 31,
                viewDistance: 50
            }
        },
        title: {
            text: ''
        },
        plotOptions: {
            series: {
                shadow: false,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        var pcnt = (this.y);
                        return Highcharts.numberFormat(pcnt) + '%';
                    }
                }
            },
            column: {
                depth: 25
            }
        },
        xAxis: {
            categories: categories,
            labels: {
                rotation: -45
            }
        },
        yAxis: {
            title:{
                enabled:false
            },
            labels: {
                formatter: function () {
                    var pcnt = (this.value);
                    return Highcharts.numberFormat(pcnt, 0, ',') + '%';
                }
            }
        },
        series: [{
            name: 'PROMIS 10 PH <br/>( % risk of hospitalization in next 6 months )',
            data: [0, 35.5, 26, 19, 16, 10, 8, 6, 2.5, 0]
        }]
    });

    $('#population-chart').highcharts({
        exporting: {
            buttons: {
                contextButton: {
                    enabled: false
                }
            }
        },
        chart: {
            type: 'pie',
            options3d: {
                enabled: true,
                alpha: 45,
                beta: 0,
            }
        },
        title: {
            text: 'PROMIS 10 PH ( % population )'
        },
        plotOptions: {
            pie: {
                depth: 25,
                slicedOffset: 20,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b><br/>{point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    },
                    connectorColor: 'silver'
                }
            }
        },
        series: [{
            data: [
                { name: '15-20', y: 3, sliced: true },
                 { name: '21-25', y: 3, sliced: true },
                 { name: '26-30', y: 4, sliced: true },
                 { name: '31-35', y: 5, sliced: true },
                 { name: '36-40', y: 6, sliced: true },
                 { name: '41-45', y: 7, sliced: true },
                 { name: '46-50', y: 11, sliced: true },
                 { name: '51-55', y: 26, sliced: true },
                 { name: '56-60', y: 32, sliced: true },
                 { name: '60 + ', y: 3, sliced: true },
            ]
        }]
    });


   
});