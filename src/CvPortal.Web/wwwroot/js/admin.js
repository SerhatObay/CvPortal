// CvPortal · Admin panel etkileşimleri
(function () {
    'use strict';

    const root = document.documentElement;

    // ---- Tema ----
    const themeBtn = document.getElementById('adminThemeToggle');
    const syncThemeIcon = () => {
        if (themeBtn) themeBtn.innerHTML = root.getAttribute('data-bs-theme') === 'dark'
            ? '<i class="bi bi-sun"></i>' : '<i class="bi bi-moon"></i>';
    };
    syncThemeIcon();
    themeBtn?.addEventListener('click', () => {
        const next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-bs-theme', next);
        try { localStorage.setItem('cv-admin-theme', next); } catch { /* yoksay */ }
        syncThemeIcon();
    });

    // ---- Mobil sidebar ----
    const sidebar = document.getElementById('adminSidebar');
    const backdrop = document.getElementById('sidebarBackdrop');
    const toggleSidebar = open => {
        sidebar?.classList.toggle('open', open);
        backdrop?.classList.toggle('show', open);
    };
    document.getElementById('sidebarToggle')?.addEventListener('click', () => toggleSidebar(true));
    backdrop?.addEventListener('click', () => toggleSidebar(false));

    // ---- Toast bildirimleri ----
    document.querySelectorAll('.toast').forEach(el => bootstrap.Toast.getOrCreateInstance(el).show());

    // ---- Silme onayı (form[data-confirm]) ----
    const modalEl = document.getElementById('confirmModal');
    if (modalEl) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        const message = document.getElementById('confirmMessage');
        let pendingForm = null;

        document.querySelectorAll('form[data-confirm]').forEach(form => {
            form.addEventListener('submit', e => {
                if (form.dataset.confirmed === 'true') return;
                e.preventDefault();
                pendingForm = form;
                message.textContent = form.dataset.confirm;
                modal.show();
            });
        });

        document.getElementById('confirmOk')?.addEventListener('click', () => {
            if (!pendingForm) return;
            pendingForm.dataset.confirmed = 'true';
            pendingForm.submit();
        });
    }

    // ---- Tablo arama & filtre ----
    const filterTable = table => {
        const term = (document.querySelector(`[data-table-search="#${table.id}"]`)?.value || '').toLocaleLowerCase('tr');
        const category = document.querySelector(`[data-table-filter="#${table.id}"]`)?.value || '';
        table.querySelectorAll('tbody tr').forEach(row => {
            const matchesText = row.textContent.toLocaleLowerCase('tr').includes(term);
            const matchesFilter = !category || row.dataset.filterValue === category;
            row.classList.toggle('d-none', !(matchesText && matchesFilter));
        });
    };
    document.querySelectorAll('[data-table-search], [data-table-filter]').forEach(input => {
        const table = document.querySelector(input.dataset.tableSearch || input.dataset.tableFilter);
        if (table) input.addEventListener('input', () => filterTable(table));
    });

    document.querySelectorAll('[data-list-search]').forEach(input => {
        const list = document.querySelector(input.dataset.listSearch);
        input.addEventListener('input', () => {
            const term = input.value.toLocaleLowerCase('tr');
            list?.querySelectorAll('[data-search-item]').forEach(item =>
                item.classList.toggle('d-none', !item.textContent.toLocaleLowerCase('tr').includes(term)));
        });
    });

    // ---- Karakter sayacı (textarea[data-counter]) ----
    document.querySelectorAll('[data-counter]').forEach(field => {
        const max = parseInt(field.dataset.counter, 10);
        const counter = document.createElement('div');
        counter.className = 'char-counter';
        field.insertAdjacentElement('afterend', counter);
        const update = () => {
            counter.textContent = `${field.value.length} / ${max}`;
            counter.classList.toggle('over', field.value.length > max);
        };
        field.addEventListener('input', update);
        update();
    });

    // ---- Görsel önizleme (input[data-preview]) ----
    document.querySelectorAll('[data-preview]').forEach(input => {
        const img = document.querySelector(input.dataset.preview);
        input.addEventListener('input', () => {
            const url = input.value.trim();
            if (!img) return;
            img.classList.toggle('d-none', !url);
            if (url) img.src = url;
        });
    });

    // ---- İkon önizleme ----
    document.querySelectorAll('[data-icon-preview]').forEach(input => {
        const target = document.querySelector(input.dataset.iconPreview);
        input.addEventListener('input', () => {
            if (target) target.innerHTML = `<i class="${input.value.trim() || 'bi bi-code-slash'}"></i>`;
        });
    });

    // ---- Gizli değeri göster / kopyala (API anahtarı) ----
    document.querySelectorAll('[data-reveal]').forEach(btn => {
        const input = document.querySelector(btn.dataset.reveal);
        btn.addEventListener('click', () => {
            if (!input) return;
            const showing = input.value === input.dataset.secret;
            input.value = showing ? input.dataset.masked : input.dataset.secret;
            btn.innerHTML = showing ? '<i class="bi bi-eye"></i>' : '<i class="bi bi-eye-slash"></i>';
        });
    });

    document.querySelectorAll('[data-copy]').forEach(btn => {
        const input = document.querySelector(btn.dataset.copy);
        btn.addEventListener('click', async () => {
            if (!input) return;
            try {
                await navigator.clipboard.writeText(input.dataset.secret || input.value);
                btn.innerHTML = '<i class="bi bi-clipboard-check"></i>';
                setTimeout(() => { btn.innerHTML = '<i class="bi bi-clipboard"></i>'; }, 1500);
            } catch { /* tarayıcı izin vermedi */ }
        });
    });

    // ---- Etiket önizleme (virgülle ayrılmış teknolojiler) ----
    document.querySelectorAll('[data-tag-preview]').forEach(input => {
        const target = document.querySelector(input.dataset.tagPreview);
        const render = () => {
            if (!target) return;
            target.innerHTML = '';
            input.value.split(/[,;]/).map(t => t.trim()).filter(Boolean).forEach(tag => {
                const span = document.createElement('span');
                span.className = 'tech-tag';
                span.textContent = tag;
                target.appendChild(span);
            });
        };
        input.addEventListener('input', render);
        render();
    });
})();
