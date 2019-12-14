// Write your JavaScript code.

var app = (function(){
    var 
        jqinit=()=>{
            $('#btnurl').on('click', () => {
                var tUrl = $('#txturl').val();

                 fetch(`/api/app`, {
                    method: 'post',
                    headers: {
                      'Accept': 'application/json',
                      'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(tUrl)
                  })
                    .then(response => response.json())
                    .then(res => {
                        console.log(res);
                    })
                    .catch(error => console.error('Unable to add item.', error));
            })
        },
        init=()=>{
            jqinit();
        };

    return { init: init };
})()