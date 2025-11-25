import re

# Read the CSS file
with open(r'c:\Users\hwang5\Projects\milinktech\website\src\FWW.Site.UI\wwwroot\css\app.css', 'r', encoding='utf-8') as f:
    css_content = f.read()

# Step 1: Update the :root section
root_section_old = r':root \{[^}]+\}'
root_section_new = ''':root {
    /* Brand Colors (theme-independent) */
    --color-primary: #0d59f2;
    --color-primary-hover: #0a47c2;
    --color-primary-dark: #0A2342;
    --color-secondary: #FF6700;
    --color-secondary-hover: #e05a00;
    
    /* Typography (theme-independent) */
    --font-family-display: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
    --font-weight-normal: 400;
    --font-weight-medium: 500;
    --font-weight-semibold: 600;
    --font-weight-bold: 700;
    --font-weight-extrabold: 800;
    --font-weight-black: 900;
    
    /* Spacing (theme-independent) */
    --spacing-xs: 0.25rem;
    --spacing-sm: 0.5rem;
    --spacing-md: 1rem;
    --spacing-lg: 1.5rem;
    --spacing-xl: 2rem;
    --spacing-2xl: 3rem;
    --spacing-3xl: 4rem;
    
    /* Border Radius (theme-independent) */
    --radius-sm: 0.25rem;
    --radius-md: 0.5rem;
    --radius-lg: 0.75rem;
    --radius-xl: 1rem;
    --radius-full: 9999px;
    
    /* Shadows (theme-independent) */
    --shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
    --shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1);
    --shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1);
    --shadow-xl: 0 20px 25px -5px rgb(0 0 0 / 0.1);
    
    /* Breakpoints (for reference) */
    --breakpoint-sm: 640px;
    --breakpoint-md: 768px;
    --breakpoint-lg: 1024px;
    --breakpoint-xl: 1280px;
    
    /* Light Mode (Default) - Based on design screenshots */
    --color-bg-primary: #ffffff;
    --color-bg-secondary: #f5f6f8;
    --color-card-bg: #ffffff;
    --color-text-primary: #333333;
    --color-text-secondary: #6b7280;
    --color-border: #e5e7eb;
    --color-header-bg: rgba(255, 255, 255, 0.8);
}

/* Dark Mode Theme */
[data-theme="dark"] {
    --color-bg-primary: #101622;
    --color-bg-secondary: #0d1117;
    --color-card-bg: #0d1117;
    --color-text-primary: #e5e7eb;
    --color-text-secondary: #9ca3af;
    --color-border: #374151;
    --color-header-bg: rgba(13, 17, 23, 0.8);
}'''

# Find and replace the :root section (multiline, dotall)
css_content = re.sub(r':root \{.*?\r?\n\}', root_section_new, css_content, count=1, flags=re.DOTALL)

# Step 2: Replace variable usages  
css_content = css_content.replace('var(--color-bg-light)', 'var(--color-bg-primary)')
css_content = css_content.replace('var(--color-card-light)', 'var(--color-card-bg)')
css_content = css_content.replace('var(--color-text-light)', 'var(--color-text-primary)')
css_content = css_content.replace('var(--color-text-gray-light)', 'var(--color-text-secondary)')
css_content = css_content.replace('var(--color-border-light)', 'var(--color-border)')

# Step 3: Remove @media (prefers-color-scheme: dark) blocks and replace with [data-theme="dark"] selectors
# Remove body dark mode media query
css_content = re.sub(
    r'/\* Dark mode support \*/\r?\n@media \(prefers-color-scheme: dark\) \{\r?\n    body \{[^}]+\}[^}]+\}',
    '',
    css_content,
    flags=re.DOTALL
)

# Remove header dark mode media query
css_content = re.sub(
    r'@media \(prefers-color-scheme: dark\) \{\r?\n    \.header \{[^}]+\}[^}]+\}',
    '',
    css_content,
    flags=re.DOTALL
)

# Remove nav-brand dark mode media query
css_content = re.sub(
    r'@media \(prefers-color-scheme: dark\) \{\r?\n    \.nav-brand \{[^}]+\}[^}]+\}',
    '\r\n\r\n[data-theme="dark"] .nav-brand {\r\n    color: white;\r\n}',
    css_content,
    flags=re.DOTALL
)

# Remove nav-link dark mode media query
css_content = re.sub(
    r'@media \(prefers-color-scheme: dark\) \{\r?\n    \.nav-link \{[^}]+\}[^}]+\}',
    '',
    css_content,
    flags=re.DOTALL
)

# Step 4: Add transition declarations
css_content = css_content.replace(
    'body {\r\n    font-family: var(--font-family-display);\r\n    font-size: 16px;\r\n    line-height: 1.5;\r\n    color: var(--color-text-primary);\r\n    background-color: var(--color-bg-primary);\r\n    -webkit-font-smoothing: antialiased;\r\n    -moz-osx-font-smoothing: grayscale;\r\n}',
    'body {\r\n    font-family: var(--font-family-display);\r\n    font-size: 16px;\r\n    line-height: 1.5;\r\n    color: var(--color-text-primary);\r\n    background-color: var(--color-bg-primary);\r\n    -webkit-font-smoothing: antialiased;\r\n    -moz-osx-font-smoothing: grayscale;\r\n    transition: background-color 0.3s ease, color 0.3s ease;\r\n}'
)

css_content = css_content.replace(
    '.header {\r\n    position: sticky;\r\n    top: 0;\r\n    z-index: 50;\r\n    background-color: rgba(255, 255, 255, 0.8);',
    '.header {\r\n    position: sticky;\r\n    top: 0;\r\n    z-index: 50;\r\n    background-color: var(--color-header-bg);'
)

css_content = css_content.replace(
    'border-bottom: 1px solid var(--color-border);',
    'border-bottom: 1px solid var(--color-border);\r\n    transition: background-color 0.3s ease, border-color 0.3s ease;',
    1  # Only first occurrence in header
)

# Write the modified content back
with open(r'c:\Users\hwang5\Projects\milinktech\website\src\FWW.Site.UI\wwwroot\css\app.css', 'w', encoding='utf-8') as f:
    f.write(css_content)

print("CSS file updated successfully!")
