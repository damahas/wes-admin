---
name: vue-hasPermi-migration
description: This skill should be used when migrating the v-hasPermi custom directive from one Vue 3 project to another. Use when users need to integrate the permission-based element visibility control from Ruoyi-style Vue applications.
---

# Vue v-hasPermi Directive Migration

This skill provides the complete workflow for migrating the v-hasPermi custom directive from a Ruoyi-style Vue 3 project to another Vue 3 project.

## Purpose

Migrate the v-hasPermi permission-based directive to control element visibility based on user permissions. The directive removes DOM elements when the user lacks the required permissions.

## When to Use This Skill

Use this skill when:
- Migrating a Ruoyi-style Vue 3 application's permission system to another project
- Implementing permission-based UI element visibility in a new Vue 3 project
- Integrating role-based access control (RBAC) with element-level permissions

## Migration Workflow

### Step 1: Gather Target Project Information

Before beginning migration, identify the target project's:
1. **State Management System**: Pinia, Vuex, or custom store
2. **Permission Data Storage**: Where user permissions are stored
3. **Build Tool**: Vite or Webpack
4. **Path Aliases**: Whether `@` alias is configured

### Step 2: Create Directory Structure

Create the following directories in the target project:
```
target-project/
├── src/
│   ├── directive/
│   │   ├── index.js
│   │   └── permission/
│   │       └── hasPermi.js
│   └── store/
│       └── modules/
│           └── user.js (if using Pinia)
```

### Step 3: Copy and Adapt Directive Files

#### Copy hasPermi.js Directive

Read the directive file from the reference and adapt it to the target project:

Reference location: `src/directive/permission/hasPermi.js`

Key adaptations required:
1. Update the user store import path to match target project structure
2. Adjust permission data access pattern if needed

#### Create index.js for Directive Registration

Create `src/directive/index.js` with directive export and registration function.

### Step 4: Setup Permission Data Store

#### Option A: Using Pinia (Recommended)

Create `src/store/modules/user.js` with permissions state:

```javascript
import { defineStore } from 'pinia'

const useUserStore = defineStore('user', {
  state: () => ({
    permissions: [] // Array of permission strings
  })
})

export default useUserStore
```

#### Option B: Adapt to Existing Store

If the target project uses a different store system:
1. Locate where user permissions are stored
2. Modify hasPermi.js to access permissions correctly
3. Ensure permissions are available when directive executes

### Step 5: Register Directive in main.js

Import and call the directive registration before mounting the app:

```javascript
import directive from './directive'

app.use(directive)
```

### Step 6: Configure Path Aliases (if needed)

If the directive uses `@` alias references, ensure the build tool configuration includes path resolution:
- For Vite: Check `vite.config.js` resolve.alias configuration
- For Webpack: Check webpack config resolve.alias

### Step 7: Test the Implementation

Create test components to verify:
1. Elements with valid permissions are displayed
2. Elements without required permissions are removed from DOM
3. Super admin permissions (`*:*:*`) work correctly
4. Multiple permission arrays work correctly

## Usage Examples

### Single Permission Check
```vue
<el-button v-hasPermi="['system:user:add']">新增</el-button>
```

### Multiple Permissions (Any Match)
```vue
<el-button v-hasPermi="['system:user:edit', 'system:user:delete']">操作</el-button>
```

## Permission Data Format

Permissions should be stored as an array of strings:
```javascript
permissions: [
  'system:user:add',
  'system:user:edit',
  'system:user:delete'
]
```

Special permissions:
- `*:*:*` - Super admin, matches all permissions

## Common Issues and Solutions

### Issue: Directive not working
- Verify directive is registered in main.js
- Check store is properly imported in hasPermi.js
- Ensure permissions array is populated when component mounts

### Issue: Elements not being removed
- Check that permissions data is available before directive execution
- Verify permission strings match exactly (case-sensitive)
- Ensure store is reactive and updates trigger directive re-evaluation

### Issue: Path resolution errors
- Confirm path aliases are configured correctly
- Update import statements to use relative paths if needed

## Assets Directory

The `assets/` directory contains:
- Template files for directive implementation
- Example main.js integration code
- Example user store configuration
