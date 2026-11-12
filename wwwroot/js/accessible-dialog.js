export function show(dialog, callback, closeOnBackdrop) {
    if (!dialog?.isConnected || dialog.open) return;

    const opener = document.activeElement;
    const listeners = new AbortController();
    const options = { signal: listeners.signal };
    let canceling = false;

    async function cancel(event) {
        event.preventDefault();
        if (canceling) return;
        canceling = true;
        try {
            await callback.invokeMethodAsync('CancelFromBrowser');
        } finally {
            canceling = false;
        }
    }

    dialog.addEventListener('cancel', cancel, options);
    if (closeOnBackdrop) {
        let startedOnBackdrop = false;
        function isBackdrop(event) {
            const bounds = dialog.getBoundingClientRect();
            return event.target === dialog && (
                event.clientX < bounds.left || event.clientX > bounds.right ||
                event.clientY < bounds.top || event.clientY > bounds.bottom
            );
        }
        dialog.addEventListener('pointerdown', event => { startedOnBackdrop = isBackdrop(event); }, options);
        dialog.addEventListener('click', event => {
            if (startedOnBackdrop && isBackdrop(event)) void cancel(event);
            startedOnBackdrop = false;
        }, options);
    }

    dialog.showModal();

    const observer = new MutationObserver(() => {
        if (dialog.isConnected) return;
        observer.disconnect();
        listeners.abort();
        if (opener?.isConnected && document.activeElement === document.body) {
            opener.focus({ preventScroll: true });
        }
    });
    observer.observe(document.body, { childList: true, subtree: true });
}
