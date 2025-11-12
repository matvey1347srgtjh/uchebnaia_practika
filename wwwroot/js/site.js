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

// Search filter panel
(function () {
    const searchForm = document.querySelector('.kp-search');
    if (!searchForm) {
        return;
    }

    const filterButton = searchForm.querySelector('.kp-search__filter');
    const filterPanel = searchForm.querySelector('#searchFilters');
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
        searchForm.submit();
    });
})();

