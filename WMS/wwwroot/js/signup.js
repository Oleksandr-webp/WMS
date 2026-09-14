const uri = 'api/users';

function createUser() {
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

    console.log(item);

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(async response => {
            console.log(response);

            if (!response.ok) {
                const problem = await response.json();
                messageField.className = "text-center alert alert-danger";
                messageField.textContent = problem.detail;
                return;
            }

            const data = await response.json();

            messageField.className = "text-center alert alert-success";
            messageField.textContent = "User created."
        })
        .catch(error => {
            console.error(error);
            messageField.className = "text-center alert alert-danger";
            messageField.textContent = "Can't connect to the server.";
        });
}