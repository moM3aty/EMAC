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

    // التنقل بين الخطوات
    nextButtons.forEach(btn => {
        btn.addEventListener('click', async (e) => {
            // إذا كان الزر هو زر إرسال النموذج (الخطوة الأخيرة)، نترك التعامل لحدث submit
            if (btn.getAttribute('type') === 'submit') return;

            const nextStep = parseInt(btn.dataset.nextStep);
            const currentStepBlock = btn.closest('.booking-step');

            // التحقق من صحة البيانات قبل الانتقال
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
            // تحديث شريط التقدم
            document.querySelectorAll('.booking-progress-bar .step').forEach(s => s.classList.remove('active'));
            for (let i = 1; i <= stepNum; i++) {
                const stepIndicator = document.querySelector(`.booking-progress-bar .step[data-step="${i}"]`);
                if (stepIndicator) stepIndicator.classList.add('active');
            }
        }
    }

    // منطق اختيار التاريخ وجلب المواعيد
    if (dateInput) {
        dateInput.min = new Date().toISOString().split("T")[0];

        dateInput.addEventListener('change', async function () {
            bookingData.date = this.value;
            slotsContainer.innerHTML = '<p class="loading-text"><i class="fas fa-spinner fa-spin"></i> جاري البحث عن المواعيد...</p>';

            console.log("جاري جلب المواعيد لـ:", bookingData); // Debugging

            try {
                // التأكد من المسار الصحيح (قد يحتاج لتعديل إذا كان التطبيق مرفوعاً على Subfolder)
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
                        slotsContainer.innerHTML = '<p class="text-warning">عذراً، جميع المواعيد محجوزة في هذا اليوم.</p>';
                    }
                } else {
                    slotsContainer.innerHTML = `<p style="color:red">خطأ: ${data.message}</p>`;
                }
            } catch (error) {
                console.error("Error fetching slots:", error);
                slotsContainer.innerHTML = '<p style="color:red">حدث خطأ في الاتصال بالخادم. يرجى المحاولة لاحقاً.</p>';
            }
        });
    }

    function selectSlot(btn, slot) {
        document.querySelectorAll('.time-slot').forEach(b => b.classList.remove('active-slot'));
        btn.classList.add('active-slot');
        bookingData.timeSlot = slot;

        // تفعيل زر التالي في الخطوة 2
        const nextBtnStep2 = document.querySelector('#step-2 .next-step-btn');
        if (nextBtnStep2) nextBtnStep2.disabled = false;
    }

    // إرسال الحجز النهائي
    const finalForm = document.getElementById('final-booking-form');
    if (finalForm) {
        finalForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            console.log("جاري إرسال الطلب..."); // Debugging

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
                    console.log("تم الحجز بنجاح:", result);
                    document.getElementById('success-req-num').textContent = result.requestNumber;
                    document.getElementById('success-dev-code').textContent = result.deviceCode;
                    goToStep(4);
                } else {
                    alert('عذراً: ' + result.message);
                    submitBtn.textContent = originalBtnText;
                    submitBtn.disabled = false;
                }
            } catch (error) {
                console.error("Booking Error:", error);
                alert('حدث خطأ غير متوقع أثناء الحجز. يرجى التحقق من اتصال الإنترنت والماولة مرة أخرى.');
                submitBtn.textContent = originalBtnText;
                submitBtn.disabled = false;
            }
        });
    }
});