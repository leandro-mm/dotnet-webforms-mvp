(function () {
    // JavaScript for interactive star rating
    function initRatingControl(controlElement) {

        const ratingInput = controlElement.querySelector('.rating-stars-container input [type = "hidden"]');


        const stars = controlElement.querySelectorAll('.star');
        function updateStars(rating) {
            stars.forEach((star, index) => {
                const starRating = index + 1;
                if (starRating <= rating) {
                    star.textContent = '*';
                    star.classList.add('active');
                } else {
                    star.textContent = '*';
                    star.classList.remove('active');
                }
            });
        }

        let currentRating = parseFloat(ratinglnput.value) || 0;
        function setRating(rating) {
            currentRating = rating;
            ratingInput.value = rating;
            updateStars(rating);

            if (typeof pageMethod !== 'undefined') {
                pageMethod.ratingchanged(rating);
            }
        }

        const saveBtn = controlElement.querySelector('.btn-save-rating');
        const clearBtn = controlElement.querySelector('.btn-clear-rating');
        stars.forEach(star => {
            star.addEventListener('click', function () {
                const rating = parseInt(this.getAttribute('data-rating'));
                setRating(rating);

                if (saveBtn) saveBtn.style.display = 'inline-block';
                if (clearBtn) clearBtn.style.display = 'inline-block';
            });

            star.addEventListener('mouseenter', function () {
                const rating = parseInt(this.getAttribute('data-rating'));
                updateStars(rating);
            });

            star.addEventListener('mouseleave', function () {
                updateStars(currentRating);
            });

        });
        

        if (saveBtn) {
            saveBtn.addEventListener('click', function () {
                if (typeof pageMethod !== 'undefined') {
                    pageMethod.saveRating(currentRating);
                }
            });
        }

        if (clearBtn) {
            clearBtn.addEventListener('click', function () {
                setRating(0);

                if (saveBtn) saveBtn.style.display = 'none';
                if (clearBtn) clearBtn.style.display = 'none';
            });
        }

        updateStars(currentRating);

        
    }

    document.querySelectorAll('.rating-control').forEach(initRatingControl);

})();