document.addEventListener('DOMContentLoaded', () => {
    let bookingData = {
        service: '',
        location: '',
        date: '',
        timeSlot: '',
        name: '',
        phone: '',
        details: ''
    };

    const nextButtons = document.querySelectorAll('.next-step-btn');
    const prevButtons = document.querySelectorAll('.prev-step-btn');
    const dateInput = document.getElementById('appointment-date');
    const slotsContainer = document.getElementById('slots-container');

    nextButtons.forEach(btn => {
        btn.addEventListener('click', async (e) => {
            if (btn.getAttribute('type') === 'submit') return;

            const nextStep = parseInt(btn.dataset.nextStep);
            const currentStepBlock = btn.closest('.booking-step');

            if (currentStepBlock.id === 'step-1') {
                const serviceSelect = document.getElementById('b-service');
                const locationSelect = document.getElementById('b-location');

                if (!serviceSelect.value || !locationSelect.value) {
                    alert('الرجاء اختيار الخدمة والموقع للمتابعة.');
                    return;
                }
                bookingData.service = serviceSelect.value;
                bookingData.location = locationSelect.value;
            }
            else if (currentStepBlock.id === 'step-2') {
                if (!bookingData.date || !bookingData.timeSlot) {
                    alert('الرجاء اختيار التاريخ والموعد المناسب.');
                    return;
                }
            }

            goToStep(nextStep);
        });
    });

    prevButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const prevStep = parseInt(btn.dataset.prevStep);
            goToStep(prevStep);
        });
    });

    function goToStep(stepNum) {
        document.querySelectorAll('.booking-step').forEach(s => s.style.display = 'none');
        const nextStepEl = document.getElementById(`step-${stepNum}`);
        if (nextStepEl) {
            nextStepEl.style.display = 'block';
            document.querySelectorAll('.booking-progress-bar .step').forEach(s => s.classList.remove('active'));
            for (let i = 1; i <= stepNum; i++) {
                const stepIndicator = document.querySelector(`.booking-progress-bar .step[data-step="${i}"]`);
                if (stepIndicator) stepIndicator.classList.add('active');
            }
        }
    }

    if (dateInput) {
        dateInput.min = new Date().toISOString().split("T")[0];

        dateInput.addEventListener('change', async function () {
            bookingData.date = this.value;
            slotsContainer.innerHTML = '<p class="loading-text"><i class="fas fa-spinner fa-spin"></i> جاري البحث عن المواعيد...</p>';

            try {
                const url = `/Booking/GetAvailableSlots?serviceType=${encodeURIComponent(bookingData.service)}&location=${encodeURIComponent(bookingData.location)}&date=${encodeURIComponent(bookingData.date)}`;

                const response = await fetch(url);
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

                const data = await response.json();

                if (data.success) {
                    slotsContainer.innerHTML = '';

                    if (data.slots && data.slots.length > 0) {
                        data.slots.forEach(slot => {
                            const btn = document.createElement('button');
                            btn.type = 'button';
                            btn.className = 'time-slot';
                            btn.textContent = slot;
                            btn.onclick = () => selectSlot(btn, slot);
                            slotsContainer.appendChild(btn);
                        });
                    } else {
                        const msg = data.message || 'عذراً، جميع المواعيد محجوزة في هذا اليوم.';
                        slotsContainer.innerHTML = `<p class="text-warning" style="color:#d9534f; font-weight:bold;">${msg}</p>`;
                    }
                } else {
                    slotsContainer.innerHTML = `<p style="color:red">خطأ: ${data.message}</p>`;
                }
            } catch (error) {
                console.error("Error fetching slots:", error);
                slotsContainer.innerHTML = '<p style="color:red">حدث خطأ في الاتصال بالخادم.</p>';
            }
        });
    }

    function selectSlot(btn, slot) {
        document.querySelectorAll('.time-slot').forEach(b => b.classList.remove('active-slot'));
        btn.classList.add('active-slot');
        bookingData.timeSlot = slot;
        const nextBtnStep2 = document.querySelector('#step-2 .next-step-btn');
        if (nextBtnStep2) nextBtnStep2.disabled = false;
    }

    const finalForm = document.getElementById('final-booking-form');
    if (finalForm) {
        finalForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            bookingData.name = document.getElementById('b-name').value;
            bookingData.phone = document.getElementById('b-phone').value;
            bookingData.details = document.getElementById('b-details').value;

            const submitBtn = finalForm.querySelector('button[type="submit"]');
            const originalBtnText = submitBtn.textContent;

            submitBtn.textContent = 'جاري الحجز...';
            submitBtn.disabled = true;

            try {
                const response = await fetch('/Booking/SubmitBooking', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        customerName: bookingData.name,
                        phoneNumber: bookingData.phone,
                        serviceType: bookingData.service,
                        location: bookingData.location,
                        appointmentDate: bookingData.date,
                        timeSlot: bookingData.timeSlot,
                        problemDescription: bookingData.details
                    })
                });

                if (!response.ok) throw new Error(`Server Error: ${response.status}`);

                const result = await response.json();

                if (result.success) {
                    document.getElementById('success-req-num').textContent = result.requestNumber;
                    document.getElementById('success-dev-code').textContent = result.deviceCode;
                    goToStep(4);

                    if (result.whatsappUrl) {
                        setTimeout(() => {
                            window.open(result.whatsappUrl, '_blank');
                        }, 1500); 
                    }

                } else {
                    alert('عذراً: ' + result.message);
                    submitBtn.textContent = originalBtnText;
                    submitBtn.disabled = false;
                }
            } catch (error) {
                alert('حدث خطأ غير متوقع أثناء الحجز.');
                submitBtn.textContent = originalBtnText;
                submitBtn.disabled = false;
            }
        });
    }
});