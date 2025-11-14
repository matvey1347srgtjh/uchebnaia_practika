// Live search suggestions for the header search bar
(function () {
    const searchForm = document.querySelector('.kp-search');
    if (!searchForm) {
        return;
    }

    const input = searchForm.querySelector('input[name="query"]');
    const suggestions = searchForm.querySelector('#searchSuggestions');
    if (!input || !suggestions) {
        return;
    }

    suggestions.dataset.hasContent = 'false';
    const suggestionsUrl = searchForm.dataset.suggestionsUrl || '/Home/SearchSuggestions';

    let abortController;
    let debounceTimer;
    let items = [];
    let activeIndex = -1;

    const clearActive = () => {
        items.forEach(item => item.setAttribute('aria-selected', 'false'));
        activeIndex = -1;
    };

    const hideSuggestions = () => {
        suggestions.classList.add('d-none');
        suggestions.innerHTML = '';
        suggestions.dataset.hasContent = 'false';
        items = [];
        clearActive();
    };

    const showSuggestions = () => {
        if (suggestions.dataset.hasContent === 'true') {
            suggestions.classList.remove('d-none');
        }
    };

    const renderSuggestions = (data) => {
        suggestions.innerHTML = '';
        suggestions.dataset.hasContent = 'false';

        if (!data || !Array.isArray(data.sections)) {
            hideSuggestions();
            return;
        }

        const fragment = document.createDocumentFragment();

        data.sections.forEach(section => {
            if (!section || !Array.isArray(section.items) || section.items.length === 0) {
                return;
            }

            const sectionEl = document.createElement('div');
            sectionEl.className = 'kp-search__section';

            if (section.title) {
                const titleEl = document.createElement('div');
                titleEl.className = 'kp-search__section-title';
                titleEl.textContent = section.title;
                sectionEl.appendChild(titleEl);
            }

            section.items.forEach(item => {
                const link = document.createElement('a');
                link.className = 'kp-search__item';
                link.href = item.detailsUrl || '#';
                link.setAttribute('role', 'option');
                link.setAttribute('tabindex', '-1');

                const posterWrapper = document.createElement('div');
                posterWrapper.className = 'kp-search__poster';
                if (item.posterUrl) {
                    const img = document.createElement('img');
                    img.src = item.posterUrl;
                    img.alt = item.title || '';
                    posterWrapper.appendChild(img);
                } else {
                    const fallback = (item.title || '?').trim().charAt(0).toUpperCase();
                    posterWrapper.textContent = fallback || '?';
                }
                link.appendChild(posterWrapper);

                const content = document.createElement('div');
                content.className = 'kp-search__content';

                const title = document.createElement('div');
                title.className = 'kp-search__title';
                title.textContent = item.title || 'Без названия';
                content.appendChild(title);

                const meta = document.createElement('div');
                meta.className = 'kp-search__meta';
                const metaParts = [];
                if (item.year) {
                    metaParts.push(item.year);
                }
                if (item.subtitle) {
                    metaParts.push(item.subtitle);
                }
                meta.textContent = metaParts.join(' • ');
                content.appendChild(meta);

                link.appendChild(content);

                const action = document.createElement('span');
                action.className = 'kp-search__action';
                const styleClass = section.actionStyle === 'secondary'
                    ? 'kp-search__action--secondary'
                    : 'kp-search__action--accent';
                action.classList.add(styleClass);
                action.textContent = section.actionText || 'Подробнее';
                link.appendChild(action);

                sectionEl.appendChild(link);
            });

            fragment.appendChild(sectionEl);
        });

        suggestions.appendChild(fragment);

        items = Array.from(suggestions.querySelectorAll('.kp-search__item'));
        if (items.length === 0) {
            hideSuggestions();
        } else {
            suggestions.dataset.hasContent = 'true';
            showSuggestions();
        }
    };

    const fetchSuggestions = async (term) => {
        if (abortController) {
            abortController.abort();
        }
        abortController = new AbortController();

        try {
            const response = await fetch(`${suggestionsUrl}?term=${encodeURIComponent(term)}`, {
                signal: abortController.signal
            });

            if (!response.ok) {
                throw new Error('Failed to load suggestions');
            }

            const data = await response.json();
            renderSuggestions(data);
        } catch (error) {
            if (error.name === 'AbortError') {
                return;
            }
            console.warn('Search suggestions error:', error);
            hideSuggestions();
        }
    };

    const handleInput = () => {
        const term = input.value.trim();
        clearTimeout(debounceTimer);

        if (term.length < 2) {
            hideSuggestions();
            return;
        }

        debounceTimer = window.setTimeout(() => {
            fetchSuggestions(term);
        }, 220);
    };

    const updateActiveItem = (newIndex) => {
        if (!items.length) {
            return;
        }

        if (activeIndex >= 0 && items[activeIndex]) {
            items[activeIndex].setAttribute('aria-selected', 'false');
        }

        activeIndex = (newIndex + items.length) % items.length;
        const current = items[activeIndex];
        current.setAttribute('aria-selected', 'true');
        current.focus({ preventScroll: true });

        const itemRect = current.getBoundingClientRect();
        const containerRect = suggestions.getBoundingClientRect();
        if (itemRect.bottom > containerRect.bottom) {
            current.scrollIntoView({ block: 'end' });
        } else if (itemRect.top < containerRect.top) {
            current.scrollIntoView({ block: 'start' });
        }
    };

    input.addEventListener('input', handleInput);
    input.addEventListener('focus', () => {
        showSuggestions();
    });

    input.addEventListener('keydown', (event) => {
        if (!items.length) {
            return;
        }

        switch (event.key) {
            case 'ArrowDown':
                event.preventDefault();
                updateActiveItem(activeIndex + 1);
                break;
            case 'ArrowUp':
                event.preventDefault();
                updateActiveItem(activeIndex - 1);
                break;
            case 'Enter':
                if (activeIndex >= 0 && items[activeIndex]) {
                    event.preventDefault();
                    window.location.href = items[activeIndex].href;
                }
                break;
            case 'Escape':
                hideSuggestions();
                break;
            default:
                break;
        }
    });

    suggestions.addEventListener('click', (event) => {
        const target = event.target.closest('.kp-search__item');
        if (!target) {
            return;
        }
        event.preventDefault();
        window.location.href = target.href;
    });

    suggestions.addEventListener('mouseover', (event) => {
        const target = event.target.closest('.kp-search__item');
        if (!target) {
            return;
        }
        clearActive();
        activeIndex = items.indexOf(target);
        if (activeIndex >= 0) {
            items[activeIndex].setAttribute('aria-selected', 'true');
        }
    });

    suggestions.addEventListener('mouseleave', () => {
        clearActive();
    });

    document.addEventListener('click', (event) => {
        if (!searchForm.contains(event.target)) {
            hideSuggestions();
        }
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') {
            hideSuggestions();
        }
    });
})();

// Hero trailer slider
(function () {
    const hero = document.querySelector('.kp-hero');
    if (!hero) {
        return;
    }

    const slides = hero.querySelectorAll('.kp-hero__slide');
    const bullets = hero.querySelectorAll('.kp-hero__bullet');
    const prevButton = hero.querySelector('.kp-hero__arrow--prev');
    const nextButton = hero.querySelector('.kp-hero__arrow--next');

    if (!slides.length) {
        return;
    }

    let currentIndex = 0;
    const videos = Array.from(hero.querySelectorAll('[data-hero-video]'));
    const volumeSliders = Array.from(hero.querySelectorAll('.kp-hero__volume'));
    let soundEnabled = false;
    let currentVolume = 0.2;
    let playbackPaused = false;
    let userPaused = false;

    const pauseAllVideos = () => {
        videos.forEach(video => {
            video.pause();
            video.currentTime = 0;
            video.muted = true;
        });
    };

    const playActiveVideo = (index) => {
        const activeSlide = slides[index];
        if (!activeSlide) {
            return;
        }

        const video = activeSlide.querySelector('[data-hero-video]');
        if (!video) {
            return;
        }

        if (playbackPaused) {
            video.pause();
            return;
        }

        video.muted = !soundEnabled;
        video.volume = soundEnabled ? currentVolume : 0;
        const playPromise = video.play();
        if (playPromise && typeof playPromise.then === 'function') {
            playPromise.catch(() => {
                // Autoplay might be blocked; fail silently
            });
        }
    };

    const setActiveSlide = (index, manualChange = false) => {
        if (index === currentIndex || index < 0 || index >= slides.length) {
            return;
        }

        slides[currentIndex].classList.remove('is-active');
        slides[currentIndex].setAttribute('aria-hidden', 'true');
        bullets[currentIndex]?.classList.remove('is-active');
        bullets[currentIndex]?.setAttribute('aria-selected', 'false');

        pauseAllVideos();

        if (manualChange && !userPaused) {
            playbackPaused = false;
        }

        currentIndex = index;
        slides[currentIndex].classList.add('is-active');
        slides[currentIndex].setAttribute('aria-hidden', 'false');
        bullets[currentIndex]?.classList.add('is-active');
        bullets[currentIndex]?.setAttribute('aria-selected', 'true');

        applySoundState();
        updatePlayButtons();
        playActiveVideo(currentIndex);
    };

    const moveToNext = (manualChange = false) => {
        const nextIndex = (currentIndex + 1) % slides.length;
        setActiveSlide(nextIndex, manualChange);
    };

    const moveToPrev = (manualChange = false) => {
        const prevIndex = (currentIndex - 1 + slides.length) % slides.length;
        setActiveSlide(prevIndex, manualChange);
    };

    bullets.forEach((bullet, index) => {
        bullet.addEventListener('click', () => {
            setActiveSlide(index, true);
        });
    });

    prevButton?.addEventListener('click', () => {
        moveToPrev(true);
    });

    nextButton?.addEventListener('click', () => {
        moveToNext(true);
    });

    const applySoundState = () => {
        videos.forEach(video => {
            video.muted = true;
            video.volume = currentVolume;
        });
        const activeSlide = slides[currentIndex];
        const video = activeSlide?.querySelector('[data-hero-video]');
        if (video) {
            video.muted = !soundEnabled;
            video.volume = soundEnabled ? currentVolume : 0;
        }
        updateSoundButtons();
        updateVolumeSliders();
    };

    const updateSoundButtons = () => {
        slides.forEach((slide, idx) => {
            const button = slide.querySelector('.kp-hero__sound-toggle');
            if (!button) {
                return;
            }
            const icon = button.querySelector('i');
            const isActive = idx === currentIndex && soundEnabled;
            button.classList.toggle('is-active', isActive);
            button.setAttribute('aria-label', isActive ? 'Выключить звук' : 'Включить звук');
            if (icon) {
                icon.classList.toggle('bi-volume-up-fill', isActive);
                icon.classList.toggle('bi-volume-mute-fill', !isActive);
            }
        });
    };

    const updatePlayButtons = () => {
        slides.forEach((slide, idx) => {
            const button = slide.querySelector('.kp-hero__play-toggle');
            if (!button) {
                return;
            }
            const icon = button.querySelector('i');
            const isPlaying = idx === currentIndex && !playbackPaused;
            button.classList.toggle('is-playing', isPlaying);
            button.setAttribute('aria-label', isPlaying ? 'Поставить на паузу' : 'Воспроизвести трейлер');
            if (icon) {
                icon.classList.toggle('bi-pause-fill', isPlaying);
                icon.classList.toggle('bi-play-fill', !isPlaying);
            }
        });
    };

    const updateVolumeSliders = () => {
        volumeSliders.forEach(slider => {
            const slideIndex = parseInt(slider.dataset.slideVolume || '-1', 10);
            if (Number.isInteger(slideIndex)) {
                slider.value = currentVolume.toString();
            }
            slider.disabled = !soundEnabled;
            slider.classList.toggle('is-disabled', !soundEnabled);

            const wrapper = slider.parentElement;
            if (wrapper) {
                wrapper.classList.toggle('is-disabled', !soundEnabled);
            }
            const track = wrapper?.querySelector('.kp-hero__volume-track');
            if (track) {
                track.style.transform = `scaleX(${soundEnabled ? currentVolume : 0})`;
            }
        });
    };

    hero.addEventListener('click', (event) => {
        const playButton = event.target.closest('.kp-hero__play-toggle');
        if (playButton && hero.contains(playButton)) {
            event.preventDefault();
            const activeSlide = slides[currentIndex];
            const video = activeSlide?.querySelector('[data-hero-video]');
            if (!video) {
                return;
            }

            if (playbackPaused) {
                playbackPaused = false;
                userPaused = false;
                playActiveVideo(currentIndex);
            } else {
                playbackPaused = true;
                userPaused = true;
                video.pause();
            }

            updatePlayButtons();
            return;
        }

        const button = event.target.closest('.kp-hero__sound-toggle');
        if (!button || !hero.contains(button)) {
            return;
        }
        event.preventDefault();

        const activeSlide = slides[currentIndex];
        const video = activeSlide?.querySelector('[data-hero-video]');
        if (!video) {
            return;
        }

        soundEnabled = !soundEnabled;
        applySoundState();

        if (soundEnabled && !playbackPaused) {
            video.currentTime = Math.max(0, video.currentTime);
            const playPromise = video.play();
            if (playPromise && typeof playPromise.then === 'function') {
                playPromise.catch(() => {});
            }
        }
    });

    volumeSliders.forEach(slider => {
        slider.addEventListener('input', (event) => {
            const target = event.target;
            const newValue = parseFloat(target.value);
            if (Number.isFinite(newValue)) {
                currentVolume = Math.max(0, Math.min(1, newValue));
                const activeSlide = slides[currentIndex];
                const video = activeSlide?.querySelector('[data-hero-video]');
                if (video) {
                    video.volume = soundEnabled && !playbackPaused ? currentVolume : 0;
                }

                const track = target.parentElement?.querySelector('.kp-hero__volume-track');
                if (track) {
                    track.style.transform = `scaleX(${soundEnabled ? currentVolume : 0})`;
                }
            }
        });
    });

    slides.forEach((slide, index) => {
        slide.id = slide.id || `kp-hero-slide-${index}`;
        if (index === 0) {
            slide.classList.add('is-active');
            slide.setAttribute('aria-hidden', 'false');
        } else {
            slide.setAttribute('aria-hidden', 'true');
        }
    });

    pauseAllVideos();
    applySoundState();
    updatePlayButtons();
    playActiveVideo(currentIndex);
    updateVolumeSliders();

    // Ensure fallback background when video ends
    slides.forEach(slide => {
        const video = slide.querySelector('[data-hero-video]');
        if (!video) {
            return;
        }

        const togglePoster = (show) => {
            const posterImage = slide.querySelector('.kp-hero__poster');
            const posterFallback = slide.querySelector('.kp-hero__poster--fallback');
            if (posterImage) {
                posterImage.style.display = show ? 'block' : '';
                posterImage.classList.toggle('is-visible', show);
            }
            if (posterFallback) {
                posterFallback.style.display = show ? 'flex' : '';
                posterFallback.classList.toggle('is-visible', show);
            }
            slide.classList.toggle('is-video-ended', show);
        };

        video.addEventListener('ended', () => {
            playbackPaused = true;
            userPaused = false;
            video.pause();
            video.currentTime = 0;
            applySoundState();
            updatePlayButtons();
            togglePoster(true);
        });

        video.addEventListener('play', () => {
            togglePoster(false);
        });
    });
})();

// Search filter panel
(function () {
    const searchForm = document.querySelector('.kp-search');
    if (!searchForm) {
        return;
    }

    const filterButton = searchForm.querySelector('.kp-search__filter');
    const filterPanel = searchForm.querySelector('#searchFilters');
    const statusContainer = document.getElementById('searchStatus');
    if (!filterButton || !filterPanel) {
        return;
    }

    const closeButton = filterPanel.querySelector('.kp-search__filters-close');
    const clearButton = filterPanel.querySelector('.kp-search__filters-clear');
    const genreSelect = filterPanel.querySelector('select[name="genre"]');
    const minInput = filterPanel.querySelector('input[name="minDuration"]');
    const maxInput = filterPanel.querySelector('input[name="maxDuration"]');

    const openFilters = () => {
        filterPanel.classList.remove('d-none');
        filterButton.setAttribute('aria-expanded', 'true');
    };

    const closeFilters = () => {
        filterPanel.classList.add('d-none');
        filterButton.setAttribute('aria-expanded', 'false');
    };

    filterButton.addEventListener('click', (event) => {
        event.preventDefault();
        if (filterPanel.classList.contains('d-none')) {
            openFilters();
        } else {
            closeFilters();
        }
    });

    closeButton?.addEventListener('click', (event) => {
        event.preventDefault();
        closeFilters();
    });

    document.addEventListener('click', (event) => {
        if (!searchForm.contains(event.target) && !filterPanel.classList.contains('d-none')) {
            closeFilters();
        }
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape' && !filterPanel.classList.contains('d-none')) {
            closeFilters();
        }
    });

    clearButton?.addEventListener('click', (event) => {
        event.preventDefault();
        if (genreSelect) {
            genreSelect.selectedIndex = 0;
        }
        if (minInput) {
            minInput.value = '';
        }
        if (maxInput) {
            maxInput.value = '';
        }
        if (statusContainer) {
            statusContainer.classList.remove('is-visible');
        }
        searchForm.classList.remove('kp-search--has-status');
        searchForm.submit();
    });
})();

