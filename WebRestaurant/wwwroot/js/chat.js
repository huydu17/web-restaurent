var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.on("ReceiveMessage", function (data) {
        console.log(data);
    })
    connection.start()
        .then(() => {
            connection.invoke('joinRoom', '@Model.Id');
        })
        .catch(err => {
            console.error(err);
        });
    var sendMessage = function (event) {
        event.preventDefault();
        var data = new FormData(event.target);
        document.getElementById('message-input').value = '';
        console.log(event);
        axios.post('/Chat/SendMessage', data)
            .then(res => {
                console.log("Message Sent!")
            })
            .catch(err => {
                console.log("Failed to send message!")
            })
    }  