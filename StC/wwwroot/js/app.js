// Write your JavaScript code.

var app = (function(){
    var
        jqinit = () => {
            $('#btnurl').on('click', () => {
                var tUrl = $('#txturl').val();
                //let bd = JSON.stringify({ url: tUrl })

                fetch(`/api/app`, {
                    method: 'post',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(`${tUrl} ${location.host} ${location.href}`)
                })
                    .then(response => response.json())
                    .then(res => {
                        console.log(res);
                        let dataArr = Object.keys(res).map(k => ({ name: k, weight: parseInt(res[k]) * 5  }));
                        wordCloud(dataArr);
                    })
                    .catch(error => console.error('Unable to add item.', error));
            })
        },

        wordCloud = (data, title) => {
            Highcharts.chart('cloudcontainer', {
                accessibility: {
                    screenReaderSection: {
                        beforeChartFormat: 'Load word Cloud'
                    }
                },
                series: [{
                    type: 'wordcloud',
                    data: data,
                    name: 'Occurrences'
                }],
                title: {
                    text: 'Wordcloud of Lorem Ipsum'
                }
            });
        },

        init=()=>{
            jqinit();
        };

    return { init: init };
})()