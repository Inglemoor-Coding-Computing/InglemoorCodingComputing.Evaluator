const defaultTheme = require('tailwindcss/defaultTheme')

module.exports = {
  content: ["./**/*.{html,js,cshtml,razor}"],
  theme: {
    extend: {
        fontFamily: {
            'sans': ['"Inter"', ...defaultTheme.fontFamily.sans],
            'mono': ['"Inconsolata"', ...defaultTheme.fontFamily.mono],
            'code': ['"Cascadia Code"']
        },
    },
},
  plugins: [],
}
