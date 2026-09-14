const uri = 'api/users';

function login() {
    const messageField = document.getElementById("message");
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    if (username.trim() === "" || password.trim() === "") {
        messageField.className = "text-center alert alert-danger";
        messageField.textContent = "Username or password are empty.";
        return;
    }

    const item = {
        username: username.trim(),
        password: password.trim()
    };

    fetch(uri + "/login", {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(async response => {
            if (!response.ok) {
                const problem = await response.json();
                messageField.className = "text-center alert alert-danger";
                messageField.textContent = problem.detail;
                return;
            }

            var data = await response.json();
            localStorage.setItem("token", data.token);
            console.log("JWT:", data.token);
            location.href = "/mainPage.html";
        })
        .catch(error => {
            console.log(error);
            messageField.className = "text-center alert alert-danger";
            messageField.textContent = "Can't connect to the server.";
        })
}