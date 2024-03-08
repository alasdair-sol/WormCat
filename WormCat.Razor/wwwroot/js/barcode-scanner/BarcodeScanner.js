let isInit = false;
let barcodeScannerReadyToUse = false;

function RunBarcodeScanner(e) {

    if (isInit == false) {
        console.log("RunBarcodeScanner: Init")
        InitBarcodeScanner(e);
    }
    else {
        console.log("RunBarcodeScanner: Scanner already Init")
    }

    if (barcodeScannerReadyToUse)
        Quagga.start();
}

function ClearBarcodeScanner() {

}



function InitBarcodeScanner(e) {
    console.log("InitBarcodeScanner: Enter")

    if (isInit === true) return;
    isInit = true;

    console.log("Setup Barcode Scanner");


    if (Quagga)
        console.log("Quagga found");

    Quagga.init({
        inputStream: {
            name: "Live",
            type: "LiveStream",
            target: document.querySelector('#interactive')    // Or '#yourElement' (optional)
        },
        decoder: {
            readers: ["ean_reader", {
                format: "ean_reader",
                config: {
                    supplements: [
                        'ean_5_reader', 'ean_2_reader'
                    ]
                }
            }]
        }
    }, function (err) {
        if (err) {
            console.log(err);
            e(err);
            return
        }
        console.log("Initialization finished. Ready to start");
        barcodeScannerReadyToUse = true;
        e(err);
    });
}


