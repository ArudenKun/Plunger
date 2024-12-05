/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./Plunger/Pages/**/*.cshtml"],
    theme: {
        extend: {},
    },
    plugins: [require('@tailwindcss/typography'), require('daisyui')],
    daisyui: {
        themes: false
    }
}

