document.addEventListener('DOMContentLoaded', () => {
    const hamburger = document.querySelector('.hamburger-menu');
    const navWrapper = document.querySelector('.main-nav-wrapper');
    const navLinks = document.querySelectorAll('.main-nav-wrapper nav ul li a');
    const header = document.querySelector('header');

    function toggleMenu() {
        if (navWrapper && hamburger) {
            navWrapper.classList.toggle('open');
            hamburger.classList.toggle('open');
            document.body.classList.toggle('no-scroll');
        }
    }

    if (hamburger) {
        hamburger.addEventListener('click', toggleMenu);
    }

    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            if (navWrapper.classList.contains('open')) {
                toggleMenu();
            }
        });
    });

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('is-visible');
            } else {
                entry.target.classList.remove('is-visible');
            }
        });
    }, {
        rootMargin: '0px',
        threshold: 0.1
    });

    document.querySelectorAll('.animate-on-scroll').forEach(element => {
        observer.observe(element);
    });

    const currentYearSpan = document.getElementById('current-year');
    if (currentYearSpan) {
        currentYearSpan.textContent = new Date().getFullYear();
    }

    function toggleHeaderBackground() {
        if (!header) return;

        if (window.location.pathname.endsWith('index.html') || window.location.pathname.endsWith('/')) {
            const scrollThreshold = 100;
            if (window.scrollY > scrollThreshold) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }
        } else {
            header.classList.add('scrolled');
        }
    }

    window.addEventListener('scroll', toggleHeaderBackground);
    toggleHeaderBackground();
});