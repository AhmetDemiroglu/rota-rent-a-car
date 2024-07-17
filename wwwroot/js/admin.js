document.addEventListener('DOMContentLoaded', () => {
    function roundToNearest(minutes) {
        const ms = 1000 * 60 * minutes;
        const now = new Date();
        const roundedDate = new Date(Math.ceil(now.getTime() / ms) * ms);
        roundedDate.setMinutes(roundedDate.getMinutes() - roundedDate.getTimezoneOffset());
        return roundedDate.toISOString().slice(0, 16);
    }

    function setInitialValues() {
        const now = new Date();
        const teslimTarih = new Date(Math.ceil(now.getTime() / (1000 * 60 * 30)) * (1000 * 60 * 30));
        const iadeTarih = new Date(teslimTarih.getTime() + (1000 * 60 * 60 * 24));
        teslimTarih.setMinutes(teslimTarih.getMinutes() - teslimTarih.getTimezoneOffset());
        iadeTarih.setMinutes(iadeTarih.getMinutes() - iadeTarih.getTimezoneOffset());

        document.getElementById('TeslimTarih').value = teslimTarih.toISOString().slice(0, 16);
        document.getElementById('IadeTarih').value = iadeTarih.toISOString().slice(0, 16);
    }

    setInitialValues();

    document.getElementById('TeslimYeriId').addEventListener('change', () => {
        const teslimYeriId = document.getElementById('TeslimYeriId').value;
        const teslimTarihi = document.getElementById('TeslimTarih').value;
        const iadeTarihi = document.getElementById('IadeTarih').value;
        if (teslimTarihi && iadeTarihi && teslimYeriId) {
            fetch(`/Rezervasyon/GetAvailableCars?teslimTarihi=${teslimTarihi}&iadeTarihi=${iadeTarihi}&teslimYeriId=${teslimYeriId}`)
                .then(response => response.json())
                .then(data => {
                    const aracSelect = document.getElementById('AracId');
                    aracSelect.innerHTML = '<option value="">Lütfen bir araç seçiniz</option>';
                    data.forEach(item => {
                        const option = document.createElement('option');
                        option.value = item.value;
                        option.text = item.text;
                        aracSelect.appendChild(option);
                    });
                })
                .catch(error => {
                    console.error("Bir hata oluştu: " + error);
                    const aracSelect = document.getElementById('AracId');
                    aracSelect.innerHTML = '<option value="">Araç yüklenemedi</option>';
                });
        }
    });

    document.getElementById('TeslimTarih').addEventListener('change', () => {
        document.getElementById('TeslimYeriId').dispatchEvent(new Event('change'));
    });

    document.getElementById('IadeTarih').addEventListener('change', () => {
        document.getElementById('TeslimYeriId').dispatchEvent(new Event('change'));
    });

    document.querySelectorAll('.box').forEach(box => {
        box.addEventListener('click', () => {
            const target = box.getAttribute('data-target');
            $('#carouselExampleIndicators').carousel(parseInt(target));
        });
    });
});
