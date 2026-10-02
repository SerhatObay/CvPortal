// CvPortal · Public site etkileşimleri
(function () {
    'use strict';

    const root = document.documentElement;

    // ---- Tema (açık / koyu) ----
    document.getElementById('themeToggle')?.addEventListener('click', () => {
        const next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-bs-theme', next);
        try { localStorage.setItem('cv-theme', next); } catch { /* gizli mod */ }
    });

    // ---- Navbar arka planı & yukarı çık butonu ----
    const nav = document.getElementById('mainNav');
    const backToTop = document.getElementById('backToTop');
    const onScroll = () => {
        const y = window.scrollY;
        nav?.classList.toggle('scrolled', y > 20);
        backToTop?.classList.toggle('show', y > 600);
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
    backToTop?.addEventListener('click', () => window.scrollTo({ top: 0 }));

    // Mobil menüde linke tıklanınca menüyü kapat
    document.querySelectorAll('#navMenu .nav-link').forEach(link => {
        link.addEventListener('click', () => {
            const menu = document.getElementById('navMenu');
            if (menu?.classList.contains('show')) bootstrap.Collapse.getOrCreateInstance(menu).hide();
        });
    });

    // ---- Scroll ile beliren animasyonlar + progress bar'lar ----
    const revealEls = document.querySelectorAll('.reveal');
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;
                entry.target.classList.add('in-view');
                observer.unobserve(entry.target);
            });
        }, { threshold: 0.12 });
        revealEls.forEach(el => observer.observe(el));
    } else {
        revealEls.forEach(el => el.classList.add('in-view'));
    }

    // ---- Sayaç animasyonu ----
    const counters = document.querySelectorAll('[data-count]');
    const animateCounter = el => {
        const target = parseInt(el.dataset.count, 10) || 0;
        const duration = 1200;
        const start = performance.now();
        const step = now => {
            const progress = Math.min((now - start) / duration, 1);
            el.textContent = Math.round(target * (1 - Math.pow(1 - progress, 3)));
            if (progress < 1) requestAnimationFrame(step);
        };
        requestAnimationFrame(step);
    };
    if ('IntersectionObserver' in window && !matchMedia('(prefers-reduced-motion: reduce)').matches) {
        const counterObserver = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;
                animateCounter(entry.target);
                counterObserver.unobserve(entry.target);
            });
        }, { threshold: 0.6 });
        counters.forEach(el => counterObserver.observe(el));
    }

    // ---- Proje filtresi ----
    document.querySelectorAll('.filter-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.filter-btn').forEach(b => b.classList.toggle('active', b === btn));
            const onlyFeatured = btn.dataset.filter === 'featured';
            document.querySelectorAll('.project-col').forEach(col => {
                col.classList.toggle('is-hidden', onlyFeatured && col.dataset.featured !== 'true');
            });
        });
    });

    // Yazdırırken tüm animasyonlu öğeleri görünür yap
    window.addEventListener('beforeprint', () => revealEls.forEach(el => el.classList.add('in-view')));
})();
