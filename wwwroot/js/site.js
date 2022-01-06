// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/********************************************************************************************/
/* Custom Scripts */
var
    container = document.getElementById('changeText_1'),
    things = ['يجب عليك تسجيل الدخول\n لمعرفه السن والجنس والتشخيص وطريقه العلاج علي اساسه\n بعد ذلك يجب ادخال الاعراض التي تظهر علي المريض كامله \n يتم معرفه المرض المصاب به علي اساس الاعراض التي تم ادخالها  \n بناءا علي معرفه المرض يتم اعطاء المريض العلاج المناسب'],
    t = -1,
    thing = '',
    message = container.innerHTML,
    mode = 'write',
    delay = 1000;
function updateText(txt) { container.innerHTML = txt; }
function tick() {
    if (container.innerHTML.length == 0) {
        t++;
        thing = things[t];
        message = '';
        mode = 'write';
    }
    switch (mode) {
        case 'write':
            message += thing.slice(0, 1);
            thing = thing.substr(1);
            updateText(message);
            if (thing.length === 0 && t === (things.length - 1)) { window.clearTimeout(timeout); return; }
            if (thing.length == 0) { mode = 'delete'; delay = 3000; }
            else { delay = 32 + Math.round(Math.random() * 40); }
            break;
        case 'delete':
            message = message.slice(0, -1);
            updateText(message);
            if (message.length == 0) { mode = 'write'; delay = 1000; }
            else { delay = 32 + Math.round(Math.random() * 100); }
            break;
    }
    timeout = window.setTimeout(tick, delay);
}
var timeout = window.setTimeout(tick, delay);