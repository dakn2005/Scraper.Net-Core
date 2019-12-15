// Write your JavaScript code.
// test: https://en.wikipedia.org/wiki/Main_Page

var app = (function(){
    var
        jqinit = () => {
            $('#btnurl').on('click', () => {
                var tUrl = $('#txturl').val();

                //let bd = JSON.stringify({ url: tUrl })

                $('#btnurl').val('loading...').attr('disabled', true);

                fetch(`/api/app`, {
                    method: 'post',
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(`${tUrl} ${new URL(tUrl).host}`)
                })
                    .then(response => response.json())
                    .then(res => {
                        $('#btnurl').removeAttr('disabled').val('Search');
                        let dataArr = Object.keys(res).map(k => ({ name: k, weight: parseInt(res[k]) * 5 }));
                        wordCloud(dataArr);
                    }).catch(error => console.error('Unable to add item.', error));
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
                    text: ''
                }
            });
        },

        readData = () => {

            fetch(`/api/app`, {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                //body: JSON.stringify(`${tUrl} ${new URL(tUrl).host}`)
            })
                .then(response => response.json())
                .then(res => {
                    console.log(res);
                    loadWords(res);
                }).catch(error => console.error('Unable to add item.', error));
        },

       

        loadWords = (arr) => {
            function wVM (dt) {
                var self = this;
                self.wordscount = dt;
            };

            ko.applyBindings(new wVM(arr));
        }
            

        init=()=>{
            jqinit();
        };

    return { init: init, getAllWords: readData };
})()