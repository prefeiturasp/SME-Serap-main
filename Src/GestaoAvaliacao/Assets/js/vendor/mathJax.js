function timerRefreshMath() {
    var counter = 0;
    function reloadMathJax() {
        clearInterval(interval);
        interval = setInterval(reloadMathJax, 500);
        counter += 1;
        MathJax.Hub.Queue(['Typeset', MathJax.Hub]);
    }
    var interval = setInterval(reloadMathJax, counter);
};

$(document).ready(function () {
    MathJax.Hub.Config({
        extensions: ['tex2jax.js', 'TeX/AMSmath.js', 'TeX/AMSsymbols.js'],
        tex2jax: { inlineMath: [['$$', '$$'], ['\\(', '\\)']] },
        jax: ['input/TeX', 'output/HTML-CSS'],
        displayAlign: 'center',
        displayIndent: '0.1em',
        showProcessingMessages: false,
    });

    MathJax.Localization.setLocale('pt-br');
    timerRefreshMath();
});
