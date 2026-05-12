function toggleSave(petId) {
    fetch('/SavedPets/Toggle', {//2) send to controller(backend c#)
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ petId: petId }) //1) js send json to 
    })
    .then(res => {
        if (res.status === 401){
            window.location.href = '/Account/Login';
            return null;
        }
        return res.json(); //convert json to js
    })
    .then(data => {
        if (!data) return;

        const button  = document.getElementById(`save-btn-${petId}`);
        const card = button.closest('.pet-card');

        if (data.saved) {
            button.classList.add('saved');
        } 
        else {
             button.classList.remove('saved');

            if (document.body.dataset.page === "savedpets") {
                card.remove();

                const remaining = document.querySelectorAll('.pet-card').length;
                if (remaining === 0) {
                    document.querySelector('.pets-grid').innerHTML =
                        "<p>You have no saved pets yet.</p>";
                }
            }
        }
    })
    .catch(err => console.error(err)); 
}