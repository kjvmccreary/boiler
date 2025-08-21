⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Failed Tests 1 ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
TestingLibraryElementError: Unable to find an element with the text: Admin. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-qd5mg9"
    >
      <span
        class="MuiCircularProgress-root MuiCircularProgress-indeterminate MuiCircularProgress-colorPrimary css-1wxecgq-MuiCircularProgress-root"
        role="progressbar"
        style="width: 40px; height: 40px;"
      >
        <svg
          class="MuiCircularProgress-svg css-54pwck-MuiCircularProgress-svg"
          viewBox="22 22 44 44"
        >
          <circle
            class="MuiCircularProgress-circle MuiCircularProgress-circleIndeterminate css-19t5dcl-MuiCircularProgress-circle"
            cx="44"
            cy="44"
            fill="none"
            r="20.2"
            stroke-width="3.6"
          />
        </svg>
      </span>
      <p
        class="MuiTypography-root MuiTypography-body1 css-bn546x-MuiTypography-root"
      >
        Loading roles for
        Test Tenant
        ...
      </p>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <div
        class="MuiBox-root css-qd5mg9"
      >
        <span
          class="MuiCircularProgress-root MuiCircularProgress-indeterminate MuiCircularProgress-colorPrimary css-1wxecgq-MuiCircularProgress-root"
          role="progressbar"
          style="width: 40px; height: 40px;"
        >
          <svg
            class="MuiCircularProgress-svg css-54pwck-MuiCircularProgress-svg"
            viewBox="22 22 44 44"
          >
            <circle
              class="MuiCircularProgress-circle MuiCircularProgress-circleIndeterminate css-19t5dcl-MuiCircularProgress-circle"
              cx="44"
              cy="44"
              fill="none"
              r="20.2"
              stroke-width="3.6"
            />
          </svg>
        </span>
        <p
          class="MuiTypography-root MuiTypography-body1 css-bn546x-MuiTypography-root"
        >
          Loading roles for
          Test Tenant
          ...
        </p>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/__tests__/roles/RoleList.test.tsx:145:11
    143|
    144|     // ✅ Wait for the roles to load and render
    145|     await waitFor(() => {
       |           ^
    146|       expect(screen.getByText('Admin')).toBeInTheDocument()
    147|     }, { timeout: 5000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[1/2]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
TestingLibraryElementError: Unable to find an element with the text: Admin. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-qd5mg9"
    >
      <span
        class="MuiCircularProgress-root MuiCircularProgress-indeterminate MuiCircularProgress-colorPrimary css-1wxecgq-MuiCircularProgress-root"
        role="progressbar"
        style="width: 40px; height: 40px;"
      >
        <svg
          class="MuiCircularProgress-svg css-54pwck-MuiCircularProgress-svg"
          viewBox="22 22 44 44"
        >
          <circle
            class="MuiCircularProgress-circle MuiCircularProgress-circleIndeterminate css-19t5dcl-MuiCircularProgress-circle"
            cx="44"
            cy="44"
            fill="none"
            r="20.2"
            stroke-width="3.6"
          />
        </svg>
      </span>
      <p
        class="MuiTypography-root MuiTypography-body1 css-bn546x-MuiTypography-root"
      >
        Loading roles for
        Test Tenant
        ...
      </p>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <div
        class="MuiBox-root css-qd5mg9"
      >
        <span
          class="MuiCircularProgress-root MuiCircularProgress-indeterminate MuiCircularProgress-colorPrimary css-1wxecgq-MuiCircularProgress-root"
          role="progressbar"
          style="width: 40px; height: 40px;"
        >
          <svg
            class="MuiCircularProgress-svg css-54pwck-MuiCircularProgress-svg"
            viewBox="22 22 44 44"
          >
            <circle
              class="MuiCircularProgress-circle MuiCircularProgress-circleIndeterminate css-19t5dcl-MuiCircularProgress-circle"
              cx="44"
              cy="44"
              fill="none"
              r="20.2"
              stroke-width="3.6"
            />
          </svg>
        </span>
        <p
          class="MuiTypography-root MuiTypography-body1 css-bn546x-MuiTypography-root"
        >
          Loading roles for
          Test Tenant
          ...
        </p>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/__tests__/roles/RoleList.test.tsx:145:11
    143|
    144|     // ✅ Wait for the roles to load and render
    145|     await waitFor(() => {
       |           ^
    146|       expect(screen.getByText('Admin')).toBeInTheDocument()
    147|     }, { timeout: 5000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[2/2]⎯

 Test Files  1 failed (1)
      Tests  1 failed | 1 passed (2)
   Start at  11:28:41
   Duration  17.45s
