import { createRouter, createWebHistory } from 'vue-router'
import Login from '@/views/Login.vue'
import Profile from '@/views/Profile.vue'
import Crops from '@/views/Crops.vue'
import FarmRecords from '@/views/FarmRecords.vue'
import Prices from '@/views/Prices.vue'
import Policies from '@/views/Policies.vue'
import Register from '@/views/Register.vue'
import Users from '@/views/Admin/Users.vue'
import Export from '@/views/Admin/Export.vue'
import PricesManagement from '@/views/Admin/PricesManagement.vue'
import Publish from '@/views/Admin/Publish.vue'

const routes = [
  { path: '/login', component: Login },
  { path: '/profile', component: Profile },
  { path: '/crops', component: Crops },
  { path: '/farm-records', component: FarmRecords },
  { path: '/prices', component: Prices },
  { path: '/policies', component: Policies },
  { path: '/register', component: Register },
  { path: '/admin/users', component: Users },
  { path: '/admin/export', component: Export },
  { path: '/admin/prices-management', component: PricesManagement },
  { path: '/admin/publish', component: Publish },
  { path: '/', redirect: '/login' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫：根据角色控制访问
router.beforeEach((to, from, next) => {
  const userStr = localStorage.getItem('user')
  const user = userStr ? JSON.parse(userStr) : null
  const role = localStorage.getItem('role')

  // 管理员页面
  if (to.path.startsWith('/admin')) {
    if (!user || role !== 'Admin') {
      return next('/login')
    }
  }
  // 农户页面
  if (
    ['/profile', '/crops', '/farm-records', '/prices', '/policies'].includes(to.path)
    && (!user || role !== 'Farmer')
  ) {
    return next('/login')
  }
  next()
})

export default router
