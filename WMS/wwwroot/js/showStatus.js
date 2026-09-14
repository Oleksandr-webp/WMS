const uri = 'api/users';
const messageField = document.getElementById("message");

checkStatus();

function checkStatus() {
    const token = localStorage.getItem("token");
    console.log(localStorage.getItem("token"));

    fetch(uri + "/me", {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`,
            "Accept": "application/json"
        }
    })
        .then(async response => {
            const data = await response.json(); 

            if (!response.ok) {
                messageField.className = "alert alert-danger";
                messageField.textContent = data.detail;
                return;
            }

            messageField.className = "alert alert-primary";
            messageField.textContent = `Hi, ${data.username}, you are ${data.role}.`;
        })
        .catch(error => {
            console.error(error);
            messageField.className = "alert alert-danger";
            messageField.textContent = "Can't connect to the server.";
        });
}