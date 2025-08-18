(function () {
    document.querySelectorAll('[data-clear-form]').forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const form = this.closest('form');
            if (!form) return;
            const baseUrl = form.getAttribute('action') || window.location.pathname;
            window.location.href = baseUrl;
        });
    });

    document.querySelectorAll('[data-edit-row]').forEach(btn => {
        btn.addEventListener('click', function () {
            const row = this.closest('.row-editable');
            if (!row) return;
            row.classList.add('editing');
            row.querySelectorAll('input,select,textarea').forEach(el => el.disabled = false);

            row.querySelector('[data-edit-row]').classList.add('d-none');
            row.querySelector('[data-cancel-row]').classList.remove('d-none');
            row.querySelector('[data-save-row]').classList.remove('d-none');
        });
    });

    document.querySelectorAll('[data-cancel-row]').forEach(btn => {
        btn.addEventListener('click', function () {
            const row = this.closest('.row-editable');
            if (!row) return;
            row.querySelectorAll('[data-orig]').forEach(el => {
                el.value = el.getAttribute('data-orig');
                if (el.type === 'checkbox') el.checked = (el.getAttribute('data-orig') === 'true');
            });

            row.classList.remove('editing');
            row.querySelectorAll('input,select,textarea').forEach(el => el.disabled = true);
            row.querySelector('[data-edit-row]').classList.remove('d-none');
            row.querySelector('[data-cancel-row]').classList.add('d-none');
            row.querySelector('[data-save-row]').classList.add('d-none');
        });
    });

    document.querySelectorAll('[data-save-row]').forEach(btn => {
        btn.addEventListener('click', function () {
            const form = this.closest('form');
            if (form) form.submit();
        });
    });
})();
