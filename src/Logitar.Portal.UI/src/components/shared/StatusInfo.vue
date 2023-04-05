<template>
  <span>
    {{ $t(dateFormat, { date: $d(date, 'medium') }) }}
    <template v-if="actor">
      {{ $t('statusDetail.by') }}
      <b-link v-if="href" :href="href" target="_blank">
        <font-awesome-icon v-if="icon" :icon="icon" />
        <user-avatar v-else-if="user" :user="user" :size="24" />
        {{ name }} <font-awesome-icon icon="external-link-alt" />
      </b-link>
      <template v-else>
        <font-awesome-icon v-if="icon" :icon="icon" />
        <user-avatar v-else-if="user" :user="user" :size="24" />
        {{ name }}
      </template>
    </template>
  </span>
</template>

<script>
import UserAvatar from '@/components/User/UserAvatar.vue'

export default {
  name: 'StatusInfo',
  components: {
    UserAvatar
  },
  props: {
    actor: {
      type: Object,
      default: null
    },
    date: {
      type: Date,
      required: true
    },
    dateFormat: {
      type: String,
      required: true
    }
  },
  computed: {
    href() {
      if (this.actor) {
        const { id, isDeleted, type } = this.actor
        if (!isDeleted) {
          switch (type) {
            case 'ApiKey':
              return `/api-keys/${id}`
            case 'User':
              return `/users/${id}`
          }
        }
      }
      return ''
    },
    icon() {
      if (this.actor) {
        const { type } = this.actor
        switch (type) {
          case 'ApiKey':
            return 'key'
          case 'System':
            return 'robot'
        }
      }
      return ''
    },
    name() {
      if (this.actor) {
        const { name, type } = this.actor
        return type === 'System' ? this.$i18n.t('system') : name
      }
      return ''
    },
    user() {
      if (this.actor) {
        const { email, name, picture, type } = this.actor
        if (type === 'User') {
          return {
            email,
            picture,
            username: name
          }
        }
      }
      return null
    }
  }
}
</script>
